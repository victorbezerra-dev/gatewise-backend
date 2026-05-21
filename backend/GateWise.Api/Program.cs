using GateWise.Api.Extensions;
using GateWise.Core.Interfaces;
using GateWise.Infrastructure.Persistence;
using GateWise.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MQTTnet.Client;


var builder = WebApplication.CreateBuilder(args);
Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISpaceRepository, SpaceRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISpaceManagerRepository, SpaceManagerRepository>();
builder.Services.AddScoped<IAccessGrantRepository, AccessGrantRepository>();
builder.Services.AddScoped<IAccessLogRepository, AccessLogRepository>();
builder.Services.AddScoped<ISpaceAccessService, SpaceAccessService>();

builder.Services.AddSignalR();

builder.Services.AddSingleton(sp =>
{
    return new MqttClientOptionsBuilder()
        .WithClientId("GateWiseBackend")
        .WithTcpServer("broker.hivemq.com", 1883)
        .WithCleanSession()
        .Build();
});

builder.Services.AddSingleton<IMqttClient>(sp =>
{
    var factory = new MqttFactory();
    return factory.CreateMqttClient();
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddSingleton<IAuthorizationHandler, GatewiseClientHandler>();
builder.Services.AddScoped<IClaimsTransformation, KeycloakClaimsTransformer>();
builder.Services.AddControllers();
builder.Services
    .AddJwtAuthentication(builder.Configuration)
    .AddCustomAuthorization()
    .AddCustomOpenApi();


var app = builder.Build();

app.MapOpenApi();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/openapi/v1.json", "GateWise API v1");
});

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});

app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;
    response.ContentType = "application/json";

    if (response.StatusCode == StatusCodes.Status401Unauthorized)
        await response.WriteAsync("""{ "error": "unauthorized", "message": "Authentication token is missing or invalid." }""");
    else if (response.StatusCode == StatusCodes.Status403Forbidden)
        await response.WriteAsync("""{ "error": "forbidden", "message": "You are not authorized to access this resource." }""");
});

app.UseCustomExceptionHandler();
app.UseCors();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<AccessConfirmationHub>("/accessconfirmationhub");

app.Run();
