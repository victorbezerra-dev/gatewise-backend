using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace GateWise.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "http://keycloak:8080/realms/master";
                options.RequireHttpsMetadata = false;
                options.UseSecurityTokenValidators = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Headers["Authorization"].FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(token) && token.StartsWith("Bearer "))
                        {
                            var rawToken = token["Bearer ".Length..];
                            Console.WriteLine($"[Auth] Token received: {rawToken}");
                        }
                        else
                        {
                            Console.WriteLine("[Auth] Missing or malformed token.");
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"[Auth] Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("[Auth] Token validated:");
                        foreach (var claim in context.Principal?.Claims ?? Enumerable.Empty<Claim>())
                        {
                            Console.WriteLine($"-> {claim.Type}: {claim.Value}");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}
