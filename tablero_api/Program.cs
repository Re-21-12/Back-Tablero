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
            var swaggerEnabled = builder.Configuration.GetValue<bool>("Swagger:Enabled", false);
            var swaggerRoutePrefix = builder.Configuration.GetValue<string>("Swagger:RoutePrefix", "swagger");
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

            // Middleware para Swagger: permitir acceso anónimo
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value ?? string.Empty;
                if (swaggerEnabled && path.StartsWith($"/{swaggerRoutePrefix}", StringComparison.OrdinalIgnoreCase))
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
                    // Si Traefik usa StripPrefix=/swagger, dejar RoutePrefix vacío
                    c.RoutePrefix = swaggerRoutePrefix == "swagger" ? "swagger" : "";
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tablero API v1");
                    c.DocumentTitle = "Tablero API - Swagger";
                });
            }

            app.MapControllers();
            app.Run();
        }
    }
}
