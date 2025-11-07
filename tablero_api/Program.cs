using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using tablero_api.Data;
using tablero_api.Extensions;
using tablero_api.Repositories;
using tablero_api.Repositories.Interfaces;
using tablero_api.Services;
using tablero_api.Services.Interfaces;
using tablero_api.Utils;

namespace tablero_api
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // 🔹 Cargar variables de entorno desde .env
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            // =====================================================
            // 🔸 CONFIGURACIÓN GENERAL
            // =====================================================

            var swaggerEnabled = builder.Configuration.GetValue<bool>("Swagger:Enabled", true);
            var urls = builder.Configuration.GetValue<string>("Urls", null);

            if (!string.IsNullOrEmpty(urls))
                builder.WebHost.UseUrls(urls);

            // =====================================================
            // 🔸 SERVICIOS BÁSICOS Y JSON
            // =====================================================

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            builder.Services.AddEndpointsApiExplorer();

            // =====================================================
            // 🔸 CONFIGURAR SWAGGER CON JWT
            // =====================================================

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Introduce tu token JWT. Ejemplo: Bearer {token}"
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

            // =====================================================
            // 🔸 INYECCIÓN DE DEPENDENCIAS
            // =====================================================

            builder.Services.AddScoped<LocalidadRepository>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));

            // 📩 Mailer Service (🚨 ESTA ES LA LÍNEA CLAVE)
            builder.Services.AddScoped<MailerService>();

            // =====================================================
            // 🔸 IMPORT SERVICE
            // =====================================================

            builder.Services.AddHttpClient("ImportService", client =>
            {
                var baseUrl = builder.Configuration.GetValue<string>("MicroServices:ImportService");

                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    var allowed = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
                    var candidate = allowed.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a) && a.Contains("import-service", StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrWhiteSpace(candidate))
                        baseUrl = candidate.TrimEnd('/');
                }

                if (string.IsNullOrWhiteSpace(baseUrl))
                    baseUrl = "http://import-service:8080";

                client.BaseAddress = new Uri(baseUrl);
            });

            builder.Services.AddSingleton(provider =>
            {
                string key = "62219311522870687600240042448129";
                string iv = "8458586964174710";
                return new CryptoHelper(key, iv);
            });

            // =====================================================
            // 🔸 BASE DE DATOS
            // =====================================================

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                       .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
            );

            // =====================================================
            // 🔸 ADMIN SERVICE
            // =====================================================

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

            // =====================================================
            // 🔸 MAILER SERVICE (HTTP CLIENT)
            // =====================================================

            var mailerBaseUrl = builder.Configuration.GetSection("MailerService")["BaseUrl"]
                                ?? "http://localhost:8080";

            builder.Services.AddHttpClient("MailerService", client =>
            {
                client.BaseAddress = new Uri(mailerBaseUrl);
            });

            // =====================================================
            // 🔸 SOCKET SERVICE
            // =====================================================

            builder.Services.Configure<SocketServiceConfig>(builder.Configuration.GetSection("SocketService"));

            builder.Services.AddHttpClient<ISocketService, SocketService>((provider, client) =>
            {
                var config = provider.GetRequiredService<IOptions<SocketServiceConfig>>().Value;
                client.BaseAddress = new Uri(config.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // =====================================================
            // 🔸 CORS
            // =====================================================

            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .SetIsOriginAllowed(origin =>
                        {
                            if (string.IsNullOrEmpty(origin)) return false;
                            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri)) return false;

                            if (string.Equals(uri.Scheme, "http", StringComparison.OrdinalIgnoreCase)
                                && string.Equals(uri.Host, "localhost", StringComparison.OrdinalIgnoreCase)
                                && uri.Port == 4200)
                                return true;

                            return allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase)
                                   || uri.Host.EndsWith(".corazondeseda.lat", StringComparison.OrdinalIgnoreCase);
                        })
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            // =====================================================
            // 🔸 AUTENTICACIÓN Y AUTORIZACIÓN (KEYCLOAK)
            // =====================================================

            builder.Services.AddKeycloakJwt(builder.Configuration);

            builder.Services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
                options.FallbackPolicy = null;
            });

            var app = builder.Build();

            // =====================================================
            // 🔸 MIGRACIONES Y SEED
            // =====================================================

            using (var scope = app.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                try
                {
                    db.Database.Migrate();
                }
                catch (InvalidOperationException ex)
                {
                    logger.LogError(ex, "EF Core detectó cambios pendientes. Crea una migración con 'dotnet ef migrations add <Name>'");
                }

                if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("SeedData", false))
                {
                    try
                    {
                        await DataSeeder.SeedAsync(db);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($" Error al aplicar seed: {ex}");
                    }
                }
            }

            // =====================================================
            // 🔸 SWAGGER UI
            // =====================================================

            if (swaggerEnabled)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tablero API v1");
                    c.RoutePrefix = string.Empty;
                    c.DocumentTitle = "Tablero API - Swagger";
                });
            }

            // =====================================================
            // 🔸 PIPELINE DE MIDDLEWARE
            // =====================================================

            app.UseRouting();
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            // Endpoint base
            app.MapGet("/", () => Results.Ok(" API funcionando correctamente")).AllowAnonymous();

            // Respuesta OPTIONS (CORS preflight)
            app.MapMethods("{*path}", new[] { "OPTIONS" }, () => Results.Ok()).AllowAnonymous();

            // =====================================================
            // 🔸 EJECUCIÓN FINAL
            // =====================================================

            app.Urls.Clear();
            // app.Urls.Add("https://localhost:7146");
            app.Urls.Add("http://localhost:5232");
            app.Run();
        }
    }
}
