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

namespace tablero_api
{
    internal class Program
    {
        private static void Main(string[] args)
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


            // CORS
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:4200",
                        "https://front-analisis-registros.netlify.app",
                        "https://proy-analisis-re2112.duckdns.org",
                        "http://frontend:4200",
                        "http://157.180.19.137:4200",
                        "http://157.180.19.137",
    "http://vmacarioe1_umg.com.gt:4200"



                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });
            // Read Keycloak settings (appsettings.keycloak.json is optional)
            builder.Configuration.AddJsonFile("appsettings.keycloak.json", optional: true, reloadOnChange: true);
            var keycloakSection = builder.Configuration.GetSection("Keycloak");
            var keycloakAuthority = keycloakSection["Authority"] ?? "http://keycloak:8080/realms/tablero";
            var keycloakClientId = keycloakSection["ClientId"] ?? "tablero-backend";

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

            app.MapGet("/", () => "API funcionando");
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Apply DB migration and seed data
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
                // Seed roles and permisos if they do not exist
                try
                {
                    // Roles list example - ensure these exist
                    var rolesToEnsure = new[] { "Admin", "Localidad", "Equipo", "Partido", "Jugador", "Cuarto", "Imagen", "Usuario", "Rol", "Permiso", "Cliente" };
                    foreach (var rn in rolesToEnsure)
                    {
                        var exists = db.Roles.Any(r => r.Nombre == rn);
                        if (!exists)
                        {
                            db.Roles.Add(new Rol { Nombre = rn, CreatedAt = DateTime.UtcNow, CreatedBy = 0 });
                        }
                    }
                    db.SaveChanges();

                    // Example permisos seeding if table empty
                    if (!db.Permisos.Any())
                    {
                        var adminRole = db.Roles.FirstOrDefault(r => r.Nombre == "Admin");
                        if (adminRole != null)
                        {
                            var perms = new[] { "Localidad_Create", "Localidad_Read", "Localidad_Update", "Localidad_Delete" };
                            foreach (var p in perms)
                            {
                                db.Permisos.Add(new Permiso { Nombre = p, Id_Rol = adminRole.Id_Rol, CreatedAt = DateTime.UtcNow, CreatedBy = 0 });
                            }
                        }
                        db.SaveChanges();
                    }
                }
                catch
                {
                    // Ignore seeding errors at startup
                }
            }

            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}