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

            // Swagger + Auth header (Bearer)
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

            // Import service (forwarder) - forwards uploaded files to the import-service container
            builder.Services.AddHttpClient<tablero_api.Services.Interfaces.IImportService, tablero_api.Services.ImportService>(client =>
            {
                var baseUrl = builder.Configuration.GetValue<string>("MicroServices:ImportService", "http://import-service:8080");
                if (!string.IsNullOrWhiteSpace(baseUrl))
                {
                    client.BaseAddress = new Uri(baseUrl);
                }
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
            // 🔸 SERVICIO HTTP ADMIN (MICROSERVICIO)
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
            // 🔸 MAILER SERVICE (OPCIONES + HTTP CLIENT)
            // =====================================================

            builder.Services.Configure<MailerServiceOptions>(builder.Configuration.GetSection("MailerService"));

            builder.Services.AddHttpClient<IMailerServiceClient, MailerServiceClient>((sp, client) =>
            {
                var cfg = sp.GetRequiredService<IOptions<MailerServiceOptions>>().Value;

                if (string.IsNullOrWhiteSpace(cfg.BaseUrl))
                    throw new InvalidOperationException("Falta MailerService:BaseUrl en configuración.");

                client.BaseAddress = new Uri(cfg.BaseUrl.TrimEnd('/'));
                client.Timeout = TimeSpan.FromSeconds(cfg.TimeoutSeconds <= 0 ? 30 : cfg.TimeoutSeconds);
            });

            // =====================================================
            // 🔸 CONFIGURAR CORS (PERMITE WILDCARDS Y ORÍGENES ESPECÍFICOS)
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

                            // Permite orígenes exactos o cualquier subdominio de corazondeseda.lat
                            return allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase)
                                   || uri.Host.EndsWith(".corazondeseda.lat", StringComparison.OrdinalIgnoreCase);
                        })
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials(); // ⚠️ Usa esto solo si frontend envía cookies o tokens con credenciales
                });
            });

            // =====================================================
            // 🔸 AUTENTICACIÓN Y AUTORIZACIÓN (KEYCLOAK)
            // =====================================================

            builder.Services.AddKeycloakJwt(builder.Configuration);

            builder.Services.AddAuthorization(options =>
            {
                // 🔒 Política global: todos los endpoints requieren autenticación
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

                // 🚪 Permitir excepciones manuales [AllowAnonymous]
                options.FallbackPolicy = null;
            });

            var app = builder.Build();

            // =====================================================
            // 🔸 MIGRACIONES Y SEED DE BASE DE DATOS
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
                    // Registrar y continuar (temporal). NO sustituye crear la migración correcta.
                    logger.LogError(ex, "EF Core detectó cambios pendientes en el modelo. Crea y commitea una migración con 'dotnet ef migrations add <Name>' y vuelve a desplegar.");
                }

                if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("SeedData", false))
                {
                    try
                    {
                        await DataSeeder.SeedAsync(db);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error al aplicar seed: {ex}");
                    }
                }
            }

            // =====================================================
            // 🔸 MIDDLEWARES GLOBALES
            // =====================================================

            app.MapGet("/", () => "✅ API funcionando correctamente").AllowAnonymous();

            // Permite acceso libre a Swagger
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

            // =====================================================
            // 🔸 SWAGGER UI
            // =====================================================

            if (swaggerEnabled)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = ""; // Para acceso directo a dominio raíz
                    c.SwaggerEndpoint("swagger/v1/swagger.json", "Tablero API v1");
                    c.DocumentTitle = "Tablero API - Swagger";
                });
            }

            // =====================================================
            // 🔸 RESPUESTA GLOBAL PARA OPTIONS (Preflight CORS)
            // =====================================================

            app.MapMethods("{*path}", new[] { "OPTIONS" }, () => Results.Ok())
               .AllowAnonymous();

            // =====================================================
            // 🔸 ENDPOINTS DE CONTROLADORES
            // =====================================================

            app.MapControllers();

            // =====================================================
            // 🔸 EJECUCIÓN FINAL
            // =====================================================

            app.Run();
        }
    }
}
