using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using tablero_api.Data;
using tablero_api.Repositories;
using tablero_api.Repositories.Interfaces;
using tablero_api.Services;
using tablero_api.Services.Interfaces;
using tablero_api.Utils;
using tablero_api.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using tablero_api.Extensions;
using Microsoft.AspNetCore.Authorization;
using DotNetEnv;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;

namespace tablero_api
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            // Configuración Swagger y URLs
            var swaggerEnabled = builder.Configuration.GetValue<bool>("Swagger:Enabled", true);
            var urls = builder.Configuration.GetValue<string>("Urls", null);
            if (!string.IsNullOrEmpty(urls))
            {
                builder.WebHost.UseUrls(urls);
            }

            // Controllers y JSON
            builder.Services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNameCaseInsensitive = true);

            builder.Services.AddEndpointsApiExplorer();

            // Swagger con JWT
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Introduce el token JWT con el prefijo 'Bearer '. Ejemplo: Bearer {token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Servicios internos
            builder.Services.AddScoped<LocalidadRepository>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton(provider =>
            {
                string key = "62219311522870687600240042448129";
                string iv = "8458586964174710";
                return new CryptoHelper(key, iv);
            });

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));

            // Admin Service HttpClient
            builder.Services.AddHttpClient("AdminService", client =>
            {
                client.BaseAddress = new Uri("http://admin-service:3000");
            });
            builder.Services.AddScoped<IAdminService, AdminService>(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("AdminService");
                var logger = provider.GetRequiredService<ILogger<AdminService>>();
                return new AdminService(httpClient, logger);
            });

            // CORS: permite orígenes exactos y patrones con wildcard (*.dominio)
            var rawOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

            // Normalizar y separar patrones
            var normalized = rawOrigins
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .Select(o => o.Trim().TrimEnd('/'))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            // Orígenes exactos (con scheme) válidos
            var exactOrigins = normalized
                .Where(o => Uri.TryCreate(o, UriKind.Absolute, out var u) && (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps))
                .ToArray();

            // Patrones wildcard, pueden venir como "*.domain" o "https://*.domain"
            var wildcardPatterns = normalized
                .Where(o => o.Contains("*"))
                .Select(p =>
                {
                    // Extraer scheme si existe
                    if (Uri.TryCreate(p.Replace("*.", ""), UriKind.Absolute, out var tmp) && (p.StartsWith("http://") || p.StartsWith("https://")))
                    {
                        var scheme = p.StartsWith("https://") ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
                        var host = p.Replace($"{scheme}://", "").Replace("*.", "");
                        return new { Host = host, Scheme = scheme };
                    }
                    // Sin scheme: aceptar ambos schemes
                    var hostOnly = p.Replace("*.", "");
                    return new { Host = hostOnly, Scheme = (string?)null };
                })
                .ToArray();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    // Usamos SetIsOriginAllowed para soportar wildcards
                    policy.SetIsOriginAllowed(origin =>
                    {
                        if (string.IsNullOrWhiteSpace(origin)) return false;
                        var originTrim = origin.TrimEnd('/');

                        // Coincidencia exacta rápida
                        if (exactOrigins.Any(e => string.Equals(e, originTrim, StringComparison.OrdinalIgnoreCase)))
                            return true;

                        // Validar URI
                        if (!Uri.TryCreate(originTrim, UriKind.Absolute, out var uri)) return false;
                        var host = uri.Host;
                        var scheme = uri.Scheme;

                        // Comprobar patrones wildcard
                        foreach (var p in wildcardPatterns)
                        {
                            // Si el patrón especifica scheme, debe coincidir
                            if (p.Scheme != null && !string.Equals(p.Scheme, scheme, StringComparison.OrdinalIgnoreCase))
                                continue;

                            if (host.EndsWith(p.Host, StringComparison.OrdinalIgnoreCase))
                                return true;
                        }

                        return false;
                    })
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                    // Si necesitas cookies desde el frontend añade .AllowCredentials();
                });
            });

            // Keycloak JWT
            builder.Services.AddKeycloakJwt(builder.Configuration);

            // Autenticación global JWT
            builder.Services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

                options.FallbackPolicy = null; // Permitir excepciones manuales (Swagger)
            });

            var app = builder.Build();

            // Migraciones y seed
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
                if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("SeedData", false))
                {
                    try
                    {
                        await DataSeeder.SeedAsync(db);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }

            app.MapGet("/", () => "API funcionando");

            // Middleware para permitir acceso a Swagger sin autenticación
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value ?? string.Empty;
                if (swaggerEnabled && path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
                {
                    await next.Invoke();
                    return;
                }
                await next.Invoke();
            });

            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();

            // Swagger
            if (swaggerEnabled)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    // Traefik hace StripPrefix /swagger, así que RoutePrefix vacío
                    c.RoutePrefix = "";
                    c.SwaggerEndpoint("swagger/v1/swagger.json", "Tablero API v1");
                    c.DocumentTitle = "Tablero API - Swagger";
                });
            }
            app.MapMethods("{*path}", new[] { "OPTIONS" }, () => Results.Ok())
           .AllowAnonymous();
            app.MapControllers();
            app.Run();
        }
    }
}
