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

namespace tablero_api
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // Configuraci�n para soportar JWT Bearer en Swagger
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
            builder.Services.AddScoped<LocalidadRepository>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton(provider =>
            {
                string key = "62219311522870687600240042448129"; // 32 chars
                string iv = "8458586964174710";                  // 16 chars
                return new CryptoHelper(key, iv);
            });

            // EF Core + SQL Server
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            // Dependencias
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));

            // Admin Service HttpClient
            builder.Services.AddHttpClient("AdminService", client =>
            {
                client.BaseAddress = new Uri("http://127.0.0.1:3000");
            });
            // Registrar el AdminService
            builder.Services.AddScoped<IAdminService, AdminService>(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient("AdminService");
                var logger = provider.GetRequiredService<ILogger<AdminService>>();
                return new AdminService(httpClient, logger);
            });

            // CORS
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });
            // Read Keycloak settings from configuration (already in appsettings.json)
            var keycloakSection = builder.Configuration.GetSection("Keycloak");
            var keycloakAuthority = keycloakSection["Authority"] ?? "http://keycloak:8080/realms/master";
            var keycloakClientId = keycloakSection["ClientId"] ?? "admin-service";

            // Authentication using Keycloak (OpenID Connect JWKS)
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = keycloakAuthority;
                options.Audience = keycloakClientId;
                options.RequireHttpsMetadata = false; // set true in production
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    NameClaimType = "preferred_username"
                };

                // Map roles from realm_access.roles into ClaimTypes.Role
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = ctx =>
                    {
                        var jwt = ctx.SecurityToken as JwtSecurityToken;
                        if (jwt != null && jwt.Payload.TryGetValue("realm_access", out var realmAccessObj))
                        {
                            try
                            {
                                using var doc = System.Text.Json.JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(realmAccessObj));
                                if (doc.RootElement.TryGetProperty("roles", out var rolesElem) && rolesElem.ValueKind == System.Text.Json.JsonValueKind.Array)
                                {
                                    var id = ctx.Principal?.Identity as System.Security.Claims.ClaimsIdentity;
                                    foreach (var r in rolesElem.EnumerateArray())
                                    {
                                        id?.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, r.GetString() ?? ""));
                                    }
                                }
                            }
                            catch { }
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Apply DB migration and seed data
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
                        // log or throw
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
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}