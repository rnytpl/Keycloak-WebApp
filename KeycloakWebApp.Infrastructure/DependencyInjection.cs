using KeycloakWebApp.Application.Data;
using KeycloakWebApp.Application.Interfaces;
using KeycloakWebApp.Infrastructure.Data;
using KeycloakWebApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;

namespace KeycloakWebApp.Infrastructure;

public static class DependencyInjection
{
    // This extension method is added to the IServiceCollection 
    public static InfrastructureBuilder AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return new InfrastructureBuilder(services, configuration);
    }

    public class InfrastructureBuilder(IServiceCollection services, IConfiguration configuration)
    {
        public InfrastructureBuilder AddDatabase()
        {
            services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

            return this;
        }

        public InfrastructureBuilder AddKeycloak()
        {
            services.AddMemoryCache();

            // A client strictly dedicated for fetching auth token
            services.AddHttpClient("KeycloakTokenClient", client =>
            {
                var authority = configuration["Keycloak:Authority"];
                var realm = configuration["Keycloak:Realm"];

                client.BaseAddress = new Uri($"{authority}/realms/{realm}/protocol/openid-connect/token");

                client.Timeout = TimeSpan.FromSeconds(15);
            });

            // A handler that intercepts requests to inject the token

            services.AddTransient<KeycloakAuthHeaderHandler>();

            services.AddHttpClient<IIdentityService, KeycloakAdminService>(client =>
            {
                var authority = configuration["Keycloak:Authority"];
                var realm = configuration["Keycloak:Realm"];

                client.BaseAddress = new Uri(authority + $"/admin/realms/{realm}/");
                client.Timeout = TimeSpan.FromSeconds(30);

                // 
            })
            .AddHttpMessageHandler<KeycloakAuthHeaderHandler>();

            return this;
        }

        public InfrastructureBuilder AddAuthenticationAndAuthorization()
        {
            // Add authentication and authorization services here
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["Keycloak:Authority"];
                    options.MetadataAddress = configuration["Keycloak:MetadataAddress"]!;
                    options.Audience = configuration["Keycloak:Audience"];
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Keycloak:Authority"],
                        ValidateAudience = true,
                        ValidAudience = configuration["Keycloak:Audience"],
                        ValidateLifetime = true,
                        RoleClaimType = ClaimTypes.Role
                    };

                    /// Add event handler to extract roles from the "realm_access" claim and add them as individual role claims
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;

                            var realmAccessClaim = claimsIdentity?.FindFirst("realm_access");

                            if (realmAccessClaim != null)
                            {
                                using var document = JsonDocument.Parse(realmAccessClaim.Value);

                                if (document.RootElement.TryGetProperty("roles", out var rolesElement))
                                {
                                    foreach (var role in rolesElement.EnumerateArray())
                                    {
                                        claimsIdentity?
                                            .AddClaim(
                                                new Claim(ClaimTypes.Role, role.GetString()!)
                                            );
                                    }
                                }
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            return this;
        }

    }
}
