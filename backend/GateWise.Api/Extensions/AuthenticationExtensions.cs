using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;

namespace GateWise.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var authority = configuration["JwtBearer:Authority"]!;
        var validIssuer = configuration["JwtBearer:ValidIssuer"] ?? authority;
        var jwksUri = configuration["JwtBearer:JwksUri"];
        var requireHttpsMetadata = configuration.GetValue("JwtBearer:RequireHttpsMetadata", false);

        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                if (string.IsNullOrWhiteSpace(jwksUri))
                {
                    options.Authority = authority;
                }

                options.RequireHttpsMetadata = requireHttpsMetadata;
                options.UseSecurityTokenValidators = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = validIssuer,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                };

                if (!string.IsNullOrWhiteSpace(jwksUri))
                {
                    options.TokenValidationParameters.IssuerSigningKeyResolver = (_, _, keyId, _) =>
                    {
                        var keys = JwksSigningKeyResolver.Resolve(jwksUri, keyId).ToArray();
                        Console.WriteLine($"[Auth] Resolved {keys.Length} signing key(s) from JWKS. kid={keyId ?? "null"}");
                        return keys;
                    };
                }

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Headers["Authorization"].FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(token) && token.StartsWith("Bearer "))
                        {
                            var rawToken = token["Bearer ".Length..];
                            LogTokenSummary(rawToken);
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

    private static void LogTokenSummary(string rawToken)
    {
        try
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(rawToken);
            Console.WriteLine($"[Auth] Token received. kid={token.Header.Kid ?? "null"} iss={token.Issuer} validToUtc={token.ValidTo:O}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Auth] Token received but could not be parsed for summary. error={ex.Message}");
        }
    }
}

internal static class JwksSigningKeyResolver
{
    private static readonly HttpClient HttpClient = new();
    private static readonly object SyncRoot = new();
    private static IReadOnlyCollection<SecurityKey> CachedKeys = Array.Empty<SecurityKey>();
    private static DateTimeOffset CacheExpiresAt = DateTimeOffset.MinValue;

    public static IEnumerable<SecurityKey> Resolve(string jwksUri, string? keyId)
    {
        var keys = GetKeys(jwksUri);

        if (!string.IsNullOrWhiteSpace(keyId))
        {
            var matchingKeys = keys.Where(key => key.KeyId == keyId).ToArray();
            if (matchingKeys.Length > 0)
            {
                return matchingKeys;
            }

            lock (SyncRoot)
            {
                CacheExpiresAt = DateTimeOffset.MinValue;
            }

            keys = GetKeys(jwksUri);
            matchingKeys = keys.Where(key => key.KeyId == keyId).ToArray();

            if (matchingKeys.Length > 0)
            {
                return matchingKeys;
            }
        }

        return keys;
    }

    private static IReadOnlyCollection<SecurityKey> GetKeys(string jwksUri)
    {
        if (CachedKeys.Count > 0 && CacheExpiresAt > DateTimeOffset.UtcNow)
        {
            return CachedKeys;
        }

        lock (SyncRoot)
        {
            if (CachedKeys.Count > 0 && CacheExpiresAt > DateTimeOffset.UtcNow)
            {
                return CachedKeys;
            }

            Console.WriteLine($"[Auth] Fetching JWKS from {jwksUri}");
            var jwksJson = HttpClient.GetStringAsync(jwksUri).GetAwaiter().GetResult();
            var jwks = new JsonWebKeySet(jwksJson);

            CachedKeys = jwks.Keys.Cast<SecurityKey>().ToArray();
            CacheExpiresAt = DateTimeOffset.UtcNow.AddMinutes(5);

            Console.WriteLine($"[Auth] JWKS loaded. keyCount={CachedKeys.Count}");
            return CachedKeys;
        }
    }
}
