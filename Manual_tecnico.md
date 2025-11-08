### Manual Tecnico
### Diagrama de arquitectura
-Mermaid
-flowchart LR
  subgraph Cliente
    A[Web (Angular / React / Mobile)] -->|HTTP / WebSocket| B[API - tablero_api (.NET 8)]
  end

  B --> C[(SQL Server 2022)]
  B --> D[Storage: Files/Images (Mounted volume)]
  B --> E[Servicio de Mail (MailerService / SMTP o HTTP)]
  B --> F[Keycloak / Identity Provider (OAuth2 / OpenID Connect)]
  B --> G[SocketService (Real-time) - SignalR / WebSockets]
  B --> H[External services (optional): CDN, FileStore, Analytics]

  style B fill:#f0f4ff,stroke:#3b82f6
  style C fill:#fff1f0
  style F fill:#fff7ed

### Estructura del Repositorio
El proyecto está organizado siguiendo las buenas prácticas de ASP.NET Core 8, con separación clara entre:
  ● Controllers (API REST)
  ● Services
  ● Repositorios
  ● DTOs / Models
  ● Migrations (Entity Framework Core)
  ● Contexto de Base de Datos
  ● Clases utilitarias
  ● Scripts de inicialización
  ● Configuración de Docker
Esta estructura facilita la escalabilidad, modularidad, mantenimiento y limpieza del backend.

### Estructura General del Repositorio
Back-Tablero/
├── .github/
│   └── workflows/
│       └── docker-publish.yml     # CI/CD: automatiza builds y publicación Docker
│
├── tablero_api/                   # Proyecto principal ASP.NET Core 8 Web API
│   ├── .config/
│   │   └── dotnet-tools.json      # Configuración de herramientas .NET (EF Core)
│   │
│   ├── Controllers/               # Endpoints de la API
│   │   ├── AnotacionController.cs
│   │   ├── AuthController.cs
│   │   ├── CorreoController.cs
│   │   ├── CuartoController.cs
│   │   ├── EquipoController.cs
│   │   ├── FaltasController.cs
│   │   ├── ImagenController.cs
│   │   ├── ImportController.cs
│   │   ├── JugadorController.cs
│   │   ├── LocalidadController.cs
│   │   ├── PartidoController.cs
│   │   ├── PermisoController.cs
│   │   ├── RolController.cs
│   │   ├── TableroController.cs
│   │   └── UsuarioController.cs
│   │
│   ├── Data/
│   │   ├── AppDbContext.cs        # ORM: DbSets + configuración EF Core
│   │   └── DataSeeder.cs          # Semilla inicial (roles, permisos, admin)
│   │
│   ├── DTOs/
│   │   ├── ImportResponse.cs
│   │   └── (todos los DTO utilizados por Controllers)
│   │
│   ├── Extensions/
│   │   └── AuthenticationExtensions.cs  # Configuración JWT + Roles
│   │
│   ├── Filters/
│   │   └── AuthorizeCheckOperationFilter.cs  # Seguridad Swagger
│   │
│   ├── Migrations/                # Entity Framework Core Migrations
│   │   ├── 20250824075611_initialCreate.*
│   │   ├── 20250914053154_authmigrate.*
│   │   ├── 20250914070710_jugadormigration.*
│   │   ├── (...)                  # Más migraciones generadas
│   │   └── AppDbContextModelSnapshot.cs
│   │
│   ├── Models/
│   │   ├── ENTITY MODELS (.cs)
│   │   │   ├── Usuario.cs
│   │   │   ├── Rol.cs
│   │   │   ├── Permiso.cs
│   │   │   ├── Equipo.cs
│   │   │   ├── Jugador.cs
│   │   │   ├── Partido.cs
│   │   │   ├── Cuarto.cs
│   │   │   ├── Tablero.cs
│   │   │   ├── Localidad.cs
│   │   │   ├── EmailMessage.cs
│   │   │   ├── EmailTemplate.cs
│   │   │   └── RefreshToken.cs
│   │   │
│   │   ├── DTOs/
│   │   │   ├── UsuarioDto.cs
│   │   │   ├── PartidoDto.cs
│   │   │   ├── LocalidadDto.cs
│   │   │   ├── JugadorDto.cs
│   │   │   ├── EquipoDto.cs
│   │   │   └── TableroDto.cs
│   │   │
│   │   ├── KeycloakOptions.cs     # Configuración de conexión con Keycloak
│   │
│   ├── Properties/
│   │   └── launchSettings.json    # Config local (SSL, puertos)
│   │
│   ├── Repositories/
│   │   ├── IRepository.cs
│   │   ├── LocalidadRepository.cs
│   │   ├── UsuarioRepository.cs
│   │   ├── TableroRepository.cs
│   │   ├── Repository.cs          # Genérico
│   │   └── Interfaces/            # Interfaces del patrón Repositorio
│
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── AdminService.cs
│   │   ├── ImportService.cs
│   │   ├── MailerClient.cs
│   │   ├── MailerServiceClient.cs
│   │   ├── Service.cs             # Base class service
│   │   ├── SocketService.cs       # Soporte para WebSockets
│   │   ├── Interfaces/            # Interfaces de servicios
│   │   └── MailerServiceOptions.cs
│
│   ├── Utils/
│   │   └── CryptoHelper.cs        # Utilidad para hash, cifrado, firmar tokens
│
│   ├── script/
│   │   └── init.sql               # Script inicial para la base de datos
│
│   ├── Dockerfile                 # Imagen backend
│   ├── .dockerignore
│   ├── .env                       # Variables backend
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Program.cs                 # Startup del proyecto
│   └── tablero_api.csproj         # Proyecto C#
│
├── docker-compose.yml             # Levanta Backend + SQL Server
└── README.MD

## Monitoreo y Logs — Back-Tablero (.NET 8)
El proyecto utiliza herramientas internas de .NET + NGINX + Docker para monitorear la actividad del backend y registrar errores.

##Logging nativo de .NET (ILogger)
La API usa el sistema integrado de logging:
private readonly ILogger<AuthController> _logger;
_logger.LogInformation("Iniciando sesión de usuario");
_logger.LogWarning("Intento fallido de autenticación");
_logger.LogError("Error crítico al procesar solicitud");
-Tipos de log soportados:
  ● Trace
  ● Debug
  ● Information
  ● Warning
  ● Error
  ● Critical
Los logs se imprimen en:
  ● Consola (modo Desarrollo)
  ● Archivos de Docker (modo Producción)
-Logging en Docker
Para ver logs del backend:
   docker logs back-tablero
Ver en tiempo real:
   docker logs -f back-tablero
-Logging de EF Core
Entity Framework registra:
  ● consultas SQL
  ● errores de migración
  ● fallos de conexión
Activado por defecto en `appsettings.Development.json.`
-Middleware de manejo de errores
ASP.NET automáticamente captura:
  ● errores 500
  ● excepciones no controladas
  ● errores de JSON malformed
Y los registra con:
`app.UseExceptionHandler("/error");`
-Monitoreo del estado del servicio
Puedes agregar (si no existe):
 -Endpoint /health
app.MapGet("/health", () => Results.Ok(new {
    status = "ok",
    uptime = Environment.TickCount64,
    timestamp = DateTime.UtcNow
}));
Devuelve:
{
  "status": "ok",
  "uptime": 3033000,
  "timestamp": "2025-11-06T06:00:00Z"
}
-WebSockets Logging (Tablero en tiempo real)
SocketService.cs puede registrar:
  ● conexiones nuevas
  ● desconexiones
  ● actualizaciones de marcador
-Logs de Correos (MailerServiceClient)
Registra:
  ● envío de plantillas de correo
  ● errores SMTP
  ● sincronizaciones de plantillas
-Logs de Seguridad
Cada vez que falla un acceso:
  ● falta de permisos
  ● token inválido
  ● token expirado

#### Detalle de microservicios y lenguajes
Todos implementados en C# (.NET 8 Web API):
# Auth (Autenticación / Autorización)
  ● Archivos clave: AuthController.cs, KeycloakOptions.cs, AuthenticationExtensions.cs, RefreshToken.cs
  ● Responsabilidad: login, register, emisión de JWT, refresh tokens, integración con Keycloak (opcional/OIDC).
# Usuario
  ● UsuarioController.cs, Usuario.cs, UsuarioDto.cs, UsuarioRepository.cs
  ● CRUD usuarios, roles asignados, gestión de refresh tokens.
# Roles y Permisos
  ● RolController.cs, PermisoController.cs, Rol.cs, Permiso.cs
  ● Gestión de roles, permisos y su asignación.
# Equipo y Jugador (Domain de deporte)
  ● EquipoController.cs, Equipo.cs, EquipoDto.cs
  ● JugadorController.cs, Jugador.cs, JugadorDto.cs
  ● CRUD equipos y jugadores, asignaciones.
# Partido, Cuarto y Tablero (Gameplay / Scoreboard)
  ● PartidoController.cs, CuartoController.cs, TableroController.cs
  ● Lógica de creación de partidos, edición de cuartos, estado del tablero en tiempo real.
# Imagenes / Media
  ● ImagenController.cs, Imagen.cs, ImagenDto.cs
  ● Upload/download de imágenes, almacenamiento en filesystem/volume o S3/Blob.
# Notificaciones / Mailer / Plantillas
  ● CorreoController.cs, MailerClient.cs, MailerServiceClient.cs, EmailTemplate.cs, EmailMessage.cs
  ● Envío de mails (plantillas sincronizadas por migración SyncEmailTemplates).
# Import / Scripts
  ● ImportController.cs, ImportService.cs, init.sql, script/
  ● Importación masiva (CSV/xlsx), respuestas ImportResponse.
# Socket / Real-Time
  ● SocketService.cs (posible uso de SignalR o WebSockets) — orquesta actualizaciones en vivo del tablero.
# Infra / Data
  ● AppDbContext.cs, DataSeeder.cs, Migrations/ (Entity Framework Core + SQL Server).

#### Cómo levantar el sistema localmente
-Requisitos
  ● .NET 8 SDK
  ● Docker Desktop + docker-compose
  ● SQL Server (local) si no usas Docker
  ● Git
# Con Docker Compose 
    Asumo que tienes un docker-compose.yml en la raíz que levanta el API y la BD. Comandos:
- desde la raíz del repo
    git clone <repo-url>
    cd Back-Tablero-main
- Crear archivo .env (ver ejemplo más abajo), luego:
    docker compose up --build
Si hay un servicio de migraciones separado:
    docker compose up --build migrate   
    docker compose up -d    
Ver logs:
    docker compose logs -f tablero_api
    docker compose logs -f db
# Local (sin Docker)
    cd tablero_api
    dotnet restore
# configurar appsettings.Development.json o variables de entorno (ConnectionStrings, JWT secret, Keycloak si aplica)
dotnet ef database update            # aplicar migraciones
dotnet run --urls "http://localhost:5000;https://localhost:5001"
También puedes abrir tablero_api.sln en Visual Studio y ejecutar en modo Debug.

#### Especificación de endpoints por microservicio
# Auth (AuthController.cs)
  ● POST /api/Auth/login — Iniciar sesión. Body: { email, password } → Response: { token, usuario, expires }
  ● POST /api/Auth/register — Registrar usuario.
  ● POST /api/Auth/refresh — Renovar token (refresh token).
  ● POST /api/Auth/logout — Logout (revocar refresh token).
# Usuario (UsuarioController.cs)
  ● GET /api/Usuario — Listar usuarios.
  ● GET /api/Usuario/{id} — Obtener usuario.
  ● POST /api/Usuario — Crear usuario.
  ● PUT /api/Usuario/{id} — Actualizar.
  ● DELETE /api/Usuario/{id} — Eliminar.
# Rol / Permiso (RolController.cs, PermisoController.cs)
  ● GET /api/Rol — Listar roles.
  ● GET /api/Permiso — Listar permisos.
  ● POST /api/Rol, POST /api/Permiso — Crear.
# Equipo / Jugador (EquipoController.cs, JugadorController.cs)
  ● GET /api/Equipo — Listar equipos.
  ● POST /api/Equipo — Crear equipo.
  ● GET /api/Jugador — Listar jugadores.
  ● POST /api/Jugador — Agregar jugador.
  ● PUT /api/Jugador/{id} — Editar jugador.
# Partido / Cuarto / Tablero (PartidoController.cs, CuartoController.cs, TableroController.cs)
  ● GET /api/Partido — Listar partidos.
  ● POST /api/Partido — Crear partido.
  ● GET /api/Tablero — Estado del tablero (posible query ?partidoId=...).
  ● POST /api/Tablero/start/{partidoId} — Empezar tablero para partido.
  ● POST /api/Cuarto — Crear / actualizar cuartos.
# Imágenes (ImagenController.cs)
  ● POST /api/Imagen — Subir imagen (multipart/form-data).
  ● GET /api/Imagen/{id} — Descargar/mostrar imagen.
  ● DELETE /api/Imagen/{id} — Borrar imagen.
# Correo / Mailer (CorreoController.cs)
  ● POST /api/Correo/send — Enviar correo.
  ● Posible endpoint para templates sync.
# Import (ImportController.cs)
  ● POST /api/Import — Subir CSV/XLSX para importar.
  ● Response: ImportResponse con resumen de filas procesadas/errores.

#### Seguridad
JWT (local)
# Emisión: AuthController valida credenciales (usuario/password) y emite:
  ● Access Token (JWT) con claims: sub, email, roles, permissions
  ● Refresh Token (persistido en BD RefreshToken) para renovar access token cuando expire.
# Verificación:
  ● AuthenticationExtensions configura JwtBearer middleware en Program.cs.
  ● Middleware valida firma (clave simétrica JWT__Key o asimétrica si usas cert).
# Autorización:
  ● [Authorize] en controladores.
  ● Autorizaciones por roles/claims con políticas o checks custom.
# Revocación:
  ● Refresh tokens guardados en BD (modelo RefreshToken) para permitir revocación/blacklist.
## OAuth / Keycloak 
KeycloakOptions.cs sugiere que existe integración OIDC:
  ● El backend puede aceptar tokens emitidos por Keycloak (AddJwtBearer apuntando a Authority).
  ● Flujo típico: Frontend redirige a Keycloak → obtiene id_token/access_token → frontend llama al API con Authorization: Bearer <token>.
  ● El API valida token con Authority (introspección o metadata endpoint .well-known/openid-configuration) y mapea roles/claims.
Ventaja: delegas auth a Keycloak y manejas roles/permisos centralizados.
## Swagger y autorización
AuthorizeCheckOperationFilter.cs indica integración con Swagger para añadir el botón Authorize y soportar Bearer token en la UI de Swagger.

#### Bibliotecas/librerías utilizadas
Estas son las librerías más probables que verás en tablero_api.csproj o que son típicas en proyectos similares:
  ● Microsoft.AspNetCore.App (base)
  ● Microsoft.EntityFrameworkCore
  ● Microsoft.EntityFrameworkCore.SqlServer
  ● Microsoft.EntityFrameworkCore.Tools
  ● Swashbuckle.AspNetCore (Swagger)
  ● Microsoft.AspNetCore.Authentication.JwtBearer
  ● System.IdentityModel.Tokens.Jwt
  ● AutoMapper / AutoMapper.Extensions.Microsoft.DependencyInjection (si usas DTOs)
  ● FluentValidation (si validas DTOs)
  ● Microsoft.Extensions.Http (HttpClientFactory para MailerClient)
  ● Newtonsoft.Json o System.Text.Json
  ● SignalR (Microsoft.AspNetCore.SignalR) si el SocketService usa SignalR
  ● MailKit o un cliente HTTP si se integra con microservicio de mail
  ● Dapper (opcional, si haces queries rápidas)
  ● Serilog o Microsoft.Extensions.Logging (logging)
  ● Swashbuckle.AspNetCore.Filters (para operation filter)

#### Posibles errores y soluciones
-Error: La API no se conecta a SQL Server
-Causa: connection string incorrecto, SQL Server no levantado.
Fix:
-Verificar .env / appsettings.Development.json.
Si usas Docker, asegúrate que el contenedor db esté listo. Ejecuta:
    docker compose logs -f db
    docker exec -it <db_container> /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong!Passw0rd"
-Ejecuta migraciones: dotnet ef database update o usa un servicio migrate en docker-compose.
-Migraciones fallan (Timeout / Login failed)
-Causa: SQL no listo cuando migraciones corren.
    Fix: agregar retry/wait-for script antes de aplicar migraciones, o usar depends_on con healthcheck en docker-compose.
-Error: 401 Unauthorized / 403 Forbidden
-Causa: token inválido, token expirado, roles no mapeados.
    Fix:
Revisar JWT secret y configuración de Issuer/Audience.
Verificar refresh tokens y endpoint /api/Auth/refresh.
-Si usas Keycloak: comprobar aud, realm y mapeo de roles en Keycloak.
-Error: CORS bloquea peticiones desde frontend
    Fix: en Program.cs añadir:
builder.Services.AddCors(options => {
  options.AddPolicy("AllowFrontend", policy =>
    policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
});
app.UseCors("AllowFrontend");
-Error: Uploads de imágenes fallan (path no encontrado)
    Fix:
Asegurar que el folder de storage esté montado como volume en Docker.
Permisos de escritura.
Validar tamaño máximo y tipos MIME en controller.
-Error: Servicios en Docker no se reinician automáticamente
    Fix: usar restart: unless-stopped en docker-compose para servicios críticos.
-Error: Email no enviado
-Causa: credenciales incorrectas o endpoint del mailer mal configurado.
    Fix:
Verificar MAILER__BASEURL y MAILER__APIKEY.
Revisar logs de MailerServiceClient.
-Error: WebSocket / SignalR desconexiones
    Fix:
Revisar timeouts y keep-alive.
Asegurar que proxies (Nginx) soporten WebSockets.
Monitorizar uso de memoria si hay muchas conexiones.

## Buenas prácticas y próximos pasos (si quieres migrar a microservicios)
  ● Separar por bounded contexts: Auth, Users, Match/Scoring, Media, Mailer — cada uno en su repo/proyecto.
  ● API Gateway (Reverse proxy): unificar rutas y autenticación externa.
  ● Event bus (RabbitMQ / Kafka) para notificaciones en tiempo real entre servicios.
  ● Kubernetes si despliegas a producción.
  ● Pruebas: unitarias (xUnit), integración y E2E.
  ● CI/CD: .github/workflows/docker-publish.yml → configurar pipelines para build+push+deploy.
