using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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
using Microsoft.AspNetCore.Http; // Necesario para HttpContext en MapFallback
using System;
using System.Net.Http;

namespace tablero_api
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            // Cargar control de Swagger y URL desde configuración
            var swaggerEnabled = builder.Configuration.GetValue<bool>("Swagger:Enabled", false);
            var swaggerRoutePrefix = builder.Configuration.GetValue<string>("Swagger:RoutePrefix", "swagger");
            var urls = builder.Configuration.GetValue<string>("Urls", null); // e.g. "http://*:5000"
            if (!string.IsNullOrEmpty(urls))
            {
                builder.WebHost.UseUrls(urls);
            }

            builder.Services.AddControllers();
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

            builder.Services.AddEndpointsApiExplorer();
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

            // Servicios y dependencias (sin cambios funcionales)
            builder.Services.AddScoped<LocalidadRepository>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton(provider =>
            {
                // NOTA: Estas claves deberían ser cargadas desde un almacén de secretos (ej. Secrets Manager) o variables de entorno
                string key = "62219311522870687600240042448129"; // 32 chars
                string iv = "8458586964174710"; // 16 chars
                return new CryptoHelper(key, iv);
            });

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));

            // Admin Service HttpClient y registro
            builder.Services.AddHttpClient("AdminService", client =>
            {
                // NOTA: El BaseAddress debe ser el nombre del servicio interno en Docker Swarm (ej. http://admin-service)
                // Usar 127.0.0.1:3000 funciona solo si el servicio es local o si estás en desarrollo.
                client.BaseAddress = new Uri("http://127.0.0.1:3000");
            });
            builder.Services.AddScoped<IAdminService, AdminService>(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("AdminService");
                var logger = provider.GetRequiredService<ILogger<AdminService>>();
                return new AdminService(httpClient, logger);
            });

            // CORS
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            // AUTH: usar extensión que encapsula la configuración Keycloak/JWKS
            builder.Services.AddKeycloakJwt(builder.Configuration);

            // Forzar autorización global: solo usuarios autenticados (JWT/Keycloak) podrán acceder.
            // Esto protege TODAS las rutas que no tienen [AllowAnonymous].
            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            var app = builder.Build();

            // Migraciones y seed
            if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("SeedData", false))
            {
                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Database.Migrate();
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
            else
            {
                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Database.Migrate();
                }
            }

            app.MapGet("/", () => "API funcionando");

            // Habilitar Swagger
            if (swaggerEnabled)
            {
                app.UseSwagger();
                // Usamos UseSwaggerUI. La configuración de Traefik con stripPrefix
                // hace que la ruta /swagger/index.html llegue al backend como /index.html
                app.UseSwaggerUI(c =>
                {
                    // Configuramos el prefijo de ruta para que SwaggerUI sepa dónde están sus recursos
                    c.RoutePrefix = swaggerRoutePrefix ?? string.Empty;
                    c.DocumentTitle = "Tablero API - Swagger";
                });
            }

            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();

            // 🛑 SOLUCIÓN AL 401: Excluir las rutas de Swagger del FallbackPolicy global
            if (swaggerEnabled)
            {
                // Construye el prefijo de ruta basado en la configuración (ej. /swagger)
                var prefix = string.IsNullOrEmpty(swaggerRoutePrefix) ? "/" : $"/{swaggerRoutePrefix}";

                // 1. Mapea la ruta base de Swagger (ej. /swagger)
                // Esto maneja la primera solicitud y redirige al index.html
                app.MapFallback(prefix, (HttpContext context) =>
                {
                    // Redirige al recurso index.html para que el middleware de Swagger lo sirva.
                    // Request.Path contiene el prefijo que Traefik no quitó (ej. /swagger).
                    context.Response.Redirect($"{context.Request.Path}/index.html");
                    return Task.CompletedTask;
                }).AllowAnonymous(); // Permite acceso anónimo.

                // 2. Mapea todos los sub-recursos estáticos de Swagger (CSS, JS, JSON)
                // Esto captura /swagger/index.html, /swagger/v1/swagger.json, etc.
                app.MapFallback($"{prefix}/{{*path}}", (HttpContext context) =>
                {
                    // No hace falta lógica aquí, solo se asegura de que estas rutas sean alcanzables de forma anónima
                    return Task.CompletedTask;
                }).AllowAnonymous(); // Permite acceso anónimo a todos los sub-recursos.
            }

            app.MapControllers();
            app.Run();
        }
    }
}