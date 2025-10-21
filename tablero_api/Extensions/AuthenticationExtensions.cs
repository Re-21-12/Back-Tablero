using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using tablero_api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace tablero_api.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddKeycloakJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var keycloak = configuration.GetSection("Keycloak").Get<KeycloakOptions>() ?? new KeycloakOptions();
            services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var authority = (keycloak.Authority ?? string.Empty).TrimEnd('/');

                options.Authority = authority;
                options.Audience = keycloak.ClientId;
                options.RequireHttpsMetadata = keycloak.RequireHttpsMetadata;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    NameClaimType = "preferred_username"
                };

                // No otros eventos ni mapeos: solo validación del JWT con metadata/JWKS del Authority
            });

            services.AddAuthorization();

            return services;
        }
    }
}