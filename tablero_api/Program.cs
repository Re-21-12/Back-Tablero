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
            // Read Keycloak settings from configuration (already in appsettings.json)
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
                // Seed roles and permisos if they do not exist (mirror the migration)
                try
                {
                    var rolesToEnsure = new[] { "Admin", "Localidad", "Equipo", "Partido", "Jugador", "Cuarto", "Imagen", "Usuario", "Rol", "Permiso", "Cliente" };
                    foreach (var rn in rolesToEnsure)
                    {
                        if (!db.Roles.Any(r => r.Nombre == rn))
                        {
                            db.Roles.Add(new Rol { Nombre = rn, CreatedAt = DateTime.UtcNow, CreatedBy = 0 });
                        }
                    }
                    db.SaveChanges();

                    // Permisos list (name, roleName) taken from migration
                    var permisosToEnsure = new (string Nombre, string RolName)[] {
                        // Admin (1)
                        ("Localidad:Agregar", "Admin"), ("Localidad:Editar", "Admin"), ("Localidad:Eliminar", "Admin"), ("Localidad:Consultar", "Admin"),
                        ("Equipo:Agregar", "Admin"), ("Equipo:Editar", "Admin"), ("Equipo:Eliminar", "Admin"), ("Equipo:Consultar", "Admin"),
                        ("Partido:Agregar", "Admin"), ("Partido:Editar", "Admin"), ("Partido:Eliminar", "Admin"), ("Partido:Consultar", "Admin"),
                        ("Jugador:Agregar", "Admin"), ("Jugador:Editar", "Admin"), ("Jugador:Eliminar", "Admin"), ("Jugador:Consultar", "Admin"),
                        ("Cuarto:Agregar", "Admin"), ("Cuarto:Editar", "Admin"), ("Cuarto:Eliminar", "Admin"), ("Cuarto:Consultar", "Admin"),
                        ("Imagen:Agregar", "Admin"), ("Imagen:Editar", "Admin"), ("Imagen:Eliminar", "Admin"), ("Imagen:Consultar", "Admin"),
                        ("Usuario:Agregar", "Admin"), ("Usuario:Editar", "Admin"), ("Usuario:Eliminar", "Admin"), ("Usuario:Consultar", "Admin"),
                        ("Rol:Agregar", "Admin"), ("Rol:Editar", "Admin"), ("Rol:Eliminar", "Admin"), ("Rol:Consultar", "Admin"),
                        ("Permiso:Agregar", "Admin"), ("Permiso:Editar", "Admin"), ("Permiso:Eliminar", "Admin"), ("Permiso:Consultar", "Admin"),

                        // Localidad (2)
                        ("Localidad:Agregar", "Localidad"), ("Localidad:Editar", "Localidad"), ("Localidad:Eliminar", "Localidad"), ("Localidad:Consultar", "Localidad"),

                        // Equipo (3)
                        ("Equipo:Agregar", "Equipo"), ("Equipo:Editar", "Equipo"), ("Equipo:Eliminar", "Equipo"), ("Equipo:Consultar", "Equipo"),

                        // Partido (4)
                        ("Partido:Agregar", "Partido"), ("Partido:Editar", "Partido"), ("Partido:Eliminar", "Partido"), ("Partido:Consultar", "Partido"),

                        // Jugador (5)
                        ("Jugador:Agregar", "Jugador"), ("Jugador:Editar", "Jugador"), ("Jugador:Eliminar", "Jugador"), ("Jugador:Consultar", "Jugador"),

                        // Cuarto (6)
                        ("Cuarto:Agregar", "Cuarto"), ("Cuarto:Editar", "Cuarto"), ("Cuarto:Eliminar", "Cuarto"), ("Cuarto:Consultar", "Cuarto"),

                        // Imagen (7)
                        ("Imagen:Agregar", "Imagen"), ("Imagen:Editar", "Imagen"), ("Imagen:Eliminar", "Imagen"), ("Imagen:Consultar", "Imagen"),

                        // Usuario (8)
                        ("Usuario:Agregar", "Usuario"), ("Usuario:Editar", "Usuario"), ("Usuario:Eliminar", "Usuario"), ("Usuario:Consultar", "Usuario"),

                        // Rol (9)
                        ("Rol:Agregar", "Rol"), ("Rol:Editar", "Rol"), ("Rol:Eliminar", "Rol"), ("Rol:Consultar", "Rol"),

                        // Permiso (10)
                        ("Permiso:Agregar", "Permiso"), ("Permiso:Editar", "Permiso"), ("Permiso:Eliminar", "Permiso"), ("Permiso:Consultar", "Permiso"),

                        // Cliente (11) - only consultas
                        ("Localidad:Consultar", "Cliente"), ("Equipo:Consultar", "Cliente"), ("Partido:Consultar", "Cliente"), ("Jugador:Consultar", "Cliente"), ("Cuarto:Consultar", "Cliente"), ("Imagen:Consultar", "Cliente")
                    };

                    foreach (var (permNombre, rolNombre) in permisosToEnsure)
                    {
                        var rol = db.Roles.FirstOrDefault(r => r.Nombre == rolNombre);
                        if (rol == null)
                            continue; // role should exist

                        var exists = db.Permisos.Any(p => p.Nombre == permNombre && p.Id_Rol == rol.Id_Rol);
                        if (!exists)
                        {
                            db.Permisos.Add(new Permiso { Nombre = permNombre, Id_Rol = rol.Id_Rol, CreatedAt = DateTime.UtcNow, CreatedBy = 0 });
                        }
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    // Log or ignore seeding errors at startup; we keep startup resilient
                    Console.WriteLine($"Seed error: {ex.Message}");
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