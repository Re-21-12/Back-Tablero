using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tablero_api.Migrations
{
    /// <inheritdoc />
    public partial class authmigrate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    Id_Permiso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.Id_Permiso);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id_Rol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id_Rol);
                });

            migrationBuilder.CreateTable(
                name: "RolPermisos",
                columns: table => new
                {
                    PermisosId_Permiso = table.Column<int>(type: "int", nullable: false),
                    RolesId_Rol = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolPermisos", x => new { x.PermisosId_Permiso, x.RolesId_Rol });
                    table.ForeignKey(
                        name: "FK_RolPermisos_Permisos_PermisosId_Permiso",
                        column: x => x.PermisosId_Permiso,
                        principalTable: "Permisos",
                        principalColumn: "Id_Permiso",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolPermisos_Roles_RolesId_Rol",
                        column: x => x.RolesId_Rol,
                        principalTable: "Roles",
                        principalColumn: "Id_Rol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id_Usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contrasena = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Id_Rol = table.Column<int>(type: "int", nullable: false),
                    RolId_Rol = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id_Usuario);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RolId_Rol",
                        column: x => x.RolId_Rol,
                        principalTable: "Roles",
                        principalColumn: "Id_Rol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolPermisos_RolesId_Rol",
                table: "RolPermisos",
                column: "RolesId_Rol");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolId_Rol",
                table: "Usuarios",
                column: "RolId_Rol");
            migrationBuilder.InsertData(
         table: "Permisos",
         columns: new[] { "Id_Permiso", "Nombre", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" },
         values: new object[,]
         {
            { 1, "Localidad:Agregar", new DateTime(2025, 9, 14, 23, 45, 55, 9419701, DateTimeKind.Unspecified), 0, null, 0 },
            { 2, "Localidad:Editar", new DateTime(2025, 9, 14, 23, 46, 13, 9277774, DateTimeKind.Unspecified), 0, null, 0 },
            { 3, "Localidad:Eliminar", new DateTime(2025, 9, 14, 23, 46, 20, 2744043, DateTimeKind.Unspecified), 0, null, 0 },
            { 4, "Localidad:Consultar", new DateTime(2025, 9, 14, 23, 46, 32, 8362285, DateTimeKind.Unspecified), 0, null, 0 },
            { 5, "Equipo:Consultar", new DateTime(2025, 9, 14, 23, 46, 41, 2582803, DateTimeKind.Unspecified), 0, null, 0 },
            { 6, "Equipo:Agregar", new DateTime(2025, 9, 14, 23, 46, 47, 5432854, DateTimeKind.Unspecified), 0, null, 0 },
            { 7, "Equipo:Editar", new DateTime(2025, 9, 14, 23, 46, 51, 7737501, DateTimeKind.Unspecified), 0, null, 0 },
            { 8, "Equipo:Eliminar", new DateTime(2025, 9, 14, 23, 47, 28, 3762625, DateTimeKind.Unspecified), 0, null, 0 },
            { 9, "Partido:Eliminar", new DateTime(2025, 9, 14, 23, 47, 34, 3939386, DateTimeKind.Unspecified), 0, null, 0 },
            { 10, "Partido:Agregar", new DateTime(2025, 9, 14, 23, 47, 38, 8656797, DateTimeKind.Unspecified), 0, null, 0 },
            { 11, "Partido:Editar", new DateTime(2025, 9, 14, 23, 47, 43, 6163400, DateTimeKind.Unspecified), 0, null, 0 },
            { 12, "Partido:Consultar", new DateTime(2025, 9, 14, 23, 47, 49, 6378561, DateTimeKind.Unspecified), 0, null, 0 },
            { 13, "Imagen:Consultar", new DateTime(2025, 9, 14, 23, 49, 28, 5848443, DateTimeKind.Unspecified), 0, null, 0 },
            { 14, "Imagen:Editar", new DateTime(2025, 9, 14, 23, 49, 32, 5469358, DateTimeKind.Unspecified), 0, null, 0 },
            { 15, "Imagen:Eliminar", new DateTime(2025, 9, 14, 23, 49, 40, 808316, DateTimeKind.Unspecified), 0, null, 0 },
            { 16, "Imagen:Crear", new DateTime(2025, 9, 14, 23, 49, 42, 6124834, DateTimeKind.Unspecified), 0, null, 0 },
            { 17, "Jugador:Crear", new DateTime(2025, 9, 14, 23, 49, 52, 519503, DateTimeKind.Unspecified), 0, null, 0 },
            { 18, "Jugador:Editar", new DateTime(2025, 9, 14, 23, 50, 12, 8958329, DateTimeKind.Unspecified), 0, null, 0 },
            { 19, "Jugador:Eliminar", new DateTime(2025, 9, 14, 23, 50, 16, 6263949, DateTimeKind.Unspecified), 0, null, 0 },
            { 20, "Jugador:Crear", new DateTime(2025, 9, 14, 23, 50, 21, 5846939, DateTimeKind.Unspecified), 0, null, 0 },
            { 21, "Cuarto:Crear", new DateTime(2025, 9, 14, 23, 51, 14, 8282236, DateTimeKind.Unspecified), 0, null, 0 },
            { 22, "Cuarto:Eliminar", new DateTime(2025, 9, 14, 23, 51, 20, 2360234, DateTimeKind.Unspecified), 0, null, 0 },
            { 23, "Cuarto:Editar", new DateTime(2025, 9, 14, 23, 51, 23, 8858195, DateTimeKind.Unspecified), 0, null, 0 },
            { 24, "Cuarto:Consultar", new DateTime(2025, 9, 14, 23, 51, 28, 2588689, DateTimeKind.Unspecified), 0, null, 0 },
            { 25, "Permiso:Consultar", new DateTime(2025, 9, 14, 23, 52, 7, 7370168, DateTimeKind.Unspecified), 0, null, 0 },
            { 26, "Permiso:Crear", new DateTime(2025, 9, 14, 23, 52, 11, 3704984, DateTimeKind.Unspecified), 0, null, 0 },
            { 27, "Permiso:Editar", new DateTime(2025, 9, 14, 23, 52, 15, 810401, DateTimeKind.Unspecified), 0, null, 0 },
            { 28, "Permiso:Eliminar", new DateTime(2025, 9, 14, 23, 52, 18, 7530707, DateTimeKind.Unspecified), 0, null, 0 },
            { 29, "Rol:Eliminar", new DateTime(2025, 9, 14, 23, 52, 23, 8015650, DateTimeKind.Unspecified), 0, null, 0 },
            { 30, "Rol:Editar", new DateTime(2025, 9, 14, 23, 52, 27, 3617279, DateTimeKind.Unspecified), 0, null, 0 },
            { 31, "Rol:Crear", new DateTime(2025, 9, 14, 23, 52, 30, 2676562, DateTimeKind.Unspecified), 0, null, 0 },
            { 32, "Rol:Consultar", new DateTime(2025, 9, 14, 23, 52, 34, 8968018, DateTimeKind.Unspecified), 0, null, 0 },
            { 33, "Usuario:Consultar", new DateTime(2025, 9, 14, 23, 52, 52, 2372608, DateTimeKind.Unspecified), 0, null, 0 },
            { 34, "Usuario:Editar", new DateTime(2025, 9, 14, 23, 52, 58, 5525708, DateTimeKind.Unspecified), 0, null, 0 },
            { 35, "Usuario:Crear", new DateTime(2025, 9, 14, 23, 53, 1, 6008117, DateTimeKind.Unspecified), 0, null, 0 },
            { 36, "Usuario:Eliminar", new DateTime(2025, 9, 14, 23, 53, 5, 7956273, DateTimeKind.Unspecified), 0, null, 0 },
            { 37, "Rol:Consultar", new DateTime(2025, 9, 14, 23, 53, 18, 4862991, DateTimeKind.Unspecified), 0, null, 0 }
         }
     );

            // Insertar rol admin
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id_Rol", "Nombre", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, "Admin", DateTime.UtcNow, 0, null, 0 }
            );

            // Insertar usuario admin (contraseña: admin123, hash de ejemplo)
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id_Usuario", "Nombre", "Contrasena", "Id_Rol", "RolId_Rol" },
                values: new object[] { 1, "admin", "$2a$11$Q9Qw8Qw8Qw8Qw8Qw8Qw8QeQw8Qw8Qw8Qw8Qw8Qw8Qw8Qw8Qw8Qw8Q", 1, 1 }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolPermisos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
