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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
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
            { 1, "Localidad:Agregar",null , 0, null, 0 },
            { 2, "Localidad:Editar", null, 0, null, 0 },
            { 3, "Localidad:Eliminar", null, 0, null, 0 },
            { 4, "Localidad:Consultar", null, 0, null, 0 },
            { 5, "Equipo:Consultar", null, 0, null, 0 },
            { 6, "Equipo:Agregar", null, 0, null, 0 },
            { 7, "Equipo:Editar", null, 0, null, 0 },
            { 8, "Equipo:Eliminar", null, 0, null, 0 },
            { 9, "Partido:Eliminar", null, 0, null, 0 },
            { 10, "Partido:Agregar", null, 0, null, 0 },
            { 11, "Partido:Editar", null, 0, null, 0 },
            { 12, "Partido:Consultar", null, 0, null, 0 },
            { 13, "Imagen:Consultar", null, 0, null, 0 },
            { 14, "Imagen:Editar", null, 0, null, 0 },
            { 15, "Imagen:Eliminar", null, 0, null, 0 },
            { 16, "Imagen:Crear", null, 0, null, 0 },
            { 17, "Jugador:Crear", null, 0, null, 0 },
            { 18, "Jugador:Editar", null, 0, null, 0 },
            { 19, "Jugador:Eliminar", null, 0, null, 0 },
            { 20, "Jugador:Crear", null, 0, null, 0 },
            { 21, "Cuarto:Crear", null, 0, null, 0 },
            { 22, "Cuarto:Eliminar", null, 0, null, 0 },
            { 23, "Cuarto:Editar", null, 0, null, 0 },
            { 24, "Cuarto:Consultar", null, 0, null, 0 },
            { 25, "Permiso:Consultar", null, 0, null, 0 },
            { 26, "Permiso:Crear", null, 0, null, 0 },
            { 27, "Permiso:Editar", null, 0, null, 0 },
            { 28, "Permiso:Eliminar", null, 0, null, 0 },
            { 29, "Rol:Eliminar", null, 0, null, 0 },
            { 30, "Rol:Editar", null, 0, null, 0 },
            { 31, "Rol:Crear", null, 0, null, 0 },
            { 32, "Rol:Consultar", null, 0, null, 0 },
            { 33, "Usuario:Consultar", null, 0, null, 0 },
            { 34, "Usuario:Editar", null, 0, null, 0 },
            { 35, "Usuario:Crear", null, 0, null, 0 },
            { 36, "Usuario:Eliminar", null, 0, null, 0 },
            { 37, "Rol:Consultar", null, 0, null, 0 }
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
