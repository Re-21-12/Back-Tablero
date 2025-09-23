using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tablero_api.Migrations
{
    /// <inheritdoc />
    public partial class fixedpmermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Primero insertar los roles ANTES de agregar la columna y constraint
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id_Rol", "Nombre", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    {  1, "Admin", DateTime.UtcNow, 0, null, 0  },
                    { 2, "Localidad", DateTime.UtcNow, 0, null, 0   },
                    { 3, "Equipo", DateTime.UtcNow, 0, null, 0  },
                    { 4, "Partido", DateTime.UtcNow, 0, null, 0  },
                    { 5, "Jugador", DateTime.UtcNow, 0, null, 0  },
                    { 6, "Cuarto", DateTime.UtcNow, 0, null, 0  },
                    { 7, "Imagen", DateTime.UtcNow, 0, null, 0 },
                    { 8, "Usuario", DateTime.UtcNow, 0, null, 0 },
                    { 9, "Rol", DateTime.UtcNow, 0, null, 0 },
                    { 10, "Permiso", DateTime.UtcNow, 0, null, 0   },
                }
            );

            // Agregar la columna Id_Rol con valor por defecto 1 (Admin)
            migrationBuilder.AddColumn<int>(
                name: "Id_Rol",
                table: "Permisos",
                type: "int",
                nullable: false,
                defaultValue: 1);

            // AHORA actualizar permisos existentes para asignarlos al rol Admin (Id_Rol = 1)
            migrationBuilder.Sql("UPDATE Permisos SET Id_Rol = 1 WHERE Id_Permiso BETWEEN 1 AND 37");

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_Id_Rol",
                table: "Permisos",
                column: "Id_Rol");

            migrationBuilder.AddForeignKey(
                name: "FK_Permisos_Roles_Id_Rol",
                table: "Permisos",
                column: "Id_Rol",
                principalTable: "Roles",
                principalColumn: "Id_Rol",
                onDelete: ReferentialAction.NoAction);

            // Insertar nuevos permisos
            migrationBuilder.InsertData(
                table: "Permisos",
                columns: new[] { "Id_Permiso", "Nombre", "CreatedAt","Id_Rol", "CreatedBy", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "Localidad:Agregar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 2, "Localidad:Editar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 3, "Localidad:Eliminar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 4, "Localidad:Consultar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 5, "Equipo:Agregar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 6, "Equipo:Editar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 7, "Equipo:Eliminar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 8, "Partido:Consultar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 9, "Partido:Agregar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 10, "Partido:Editar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 11, "Partido:Eliminar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 12, "Jugador:Agregar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 13, "Jugador:Editar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 14, "Jugador:Eliminar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 15, "Jugador:Consultar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 16, "Cuarto:Agregar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 17, "Cuarto:Editar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 18, "Cuarto:Eliminar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 19, "Cuarto:Consultar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 20, "Imagen:Agregar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 21, "Imagen:Editar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 22, "Imagen:Consultar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 23, "Imagen:Eliminar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 24, "Usuario:Agregar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 25, "Usuario:Editar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 26, "Usuario:Eliminar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 27, "Usuario:Consultar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 28, "Rol:Agregar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 29, "Rol:Editar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 30, "Rol:Consultar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 31, "Rol:Eliminar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 32, "Permiso:Agregar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 33, "Permiso:Editar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 34, "Permiso:Consultar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 35, "Permiso:Eliminar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 36, "Localidad:Agregar", DateTime.UtcNow, 2, 0, null, 0 },
                    { 37, "Localidad:Editar", DateTime.UtcNow, 2, 0, null, 0 },
                    { 38, "Localidad:Eliminar", DateTime.UtcNow, 2, 0, null, 0 },
                    { 39, "Localidad:Consultar", DateTime.UtcNow, 2, 0, null, 0 },
                    { 40, "Equipo:Agregar", DateTime.UtcNow, 3, 0, null, 0 },
                    { 41, "Equipo:Editar", DateTime.UtcNow, 3, 0, null, 0 },
                    { 42, "Equipo:Eliminar", DateTime.UtcNow, 3, 0, null, 0 },
                    { 43, "Equipo:Consultar", DateTime.UtcNow, 3, 0, null, 0 },
                    { 44, "Partido:Agregar", DateTime.UtcNow, 4, 0, null, 0 },
                    { 45, "Partido:Editar", DateTime.UtcNow, 4, 0, null, 0 },
                    { 46, "Partido:Eliminar", DateTime.UtcNow, 4, 0, null, 0 },
                    { 47, "Partido:Consultar", DateTime.UtcNow, 4, 0, null, 0 },
                    { 48, "Jugador:Crear", DateTime.UtcNow, 5, 0, null, 0 },
                    { 49, "Jugador:Editar", DateTime.UtcNow, 5, 0, null, 0 },
                    { 50, "Jugador:Eliminar", DateTime.UtcNow, 5, 0, null, 0 },
                    { 51, "Jugador:Consultar", DateTime.UtcNow, 5, 0, null, 0 },
                    { 52, "Cuarto:Crear", DateTime.UtcNow, 6, 0, null, 0 },
                    { 53, "Cuarto:Editar", DateTime.UtcNow, 6, 0, null, 0 },
                    { 54, "Cuarto:Eliminar", DateTime.UtcNow, 6, 0, null, 0 },
                    { 55, "Cuarto:Consultar", DateTime.UtcNow, 6, 0, null, 0 },
                    { 56, "Imagen:Crear", DateTime.UtcNow, 7, 0, null, 0 },
                    { 57, "Imagen:Editar", DateTime.UtcNow, 7, 0, null, 0 },
                    { 58, "Imagen:Eliminar", DateTime.UtcNow, 7, 0, null, 0 },
                    { 59, "Imagen:Consultar", DateTime.UtcNow, 7, 0, null, 0 },
                    {60, "Usuario: Agregar", DateTime.UtcNow,8, 0, null, 0 },
                    {61, "Usuario: Editar", DateTime.UtcNow, 8, 0, null, 0 },
                    {62, "Usuario: Eliminar", DateTime.UtcNow, 8, 0, null, 0 },
                    {63, "Usuario: Consultar", DateTime.UtcNow, 8, 0, null, 0 },
                    {64, "Rol: Agregar", DateTime.UtcNow, 9, 0, null, 0 },
                    {65, "Rol: Editar", DateTime.UtcNow, 9, 0, null, 0 },
                    {66, "Rol: Eliminar", DateTime.UtcNow, 9, 0, null, 0 },
                    {67, "Rol: Consultar", DateTime.UtcNow, 9, 0, null, 0 },
                    {68, "Permiso: Agregar", DateTime.UtcNow, 10, 0, null, 0 },
                    {69, "Permiso: Editar", DateTime.UtcNow, 10, 0, null, 0 },
                    {70, "Permiso: Eliminar", DateTime.UtcNow, 10, 0, null, 0 },
                    {71, "Permiso: Consultar", DateTime.UtcNow, 10, 0, null, 0 }
                }
            );

            // Insertar usuario admin (contraseña: admin123, hash de ejemplo)
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id_Usuario", "Nombre", "Contrasena", "Id_Rol", },
                values: new object[] { 1, "admin", "$2a$11$Q9Qw8Qw8Qw8Qw8Qw8Qw8QeQw8Qw8Qw8Qw8Qw8Qw8Qw8Qw8Qw8Qw8Q", 1}
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permisos_Roles_Id_Rol",
                table: "Permisos");

            migrationBuilder.DropIndex(
                name: "IX_Permisos_Id_Rol",
                table: "Permisos");

            migrationBuilder.DropColumn(
                name: "Id_Rol",
                table: "Permisos");

            migrationBuilder.CreateTable(
                name: "PermisoRol",
                columns: table => new
                {
                    PermisosId_Permiso = table.Column<int>(type: "int", nullable: true),
                    RolesId_Rol = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_PermisoRol_Permisos_PermisosId_Permiso",
                        column: x => x.PermisosId_Permiso,
                        principalTable: "Permisos",
                        principalColumn: "Id_Permiso",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermisoRol_Roles_RolesId_Rol",
                        column: x => x.RolesId_Rol,
                        principalTable: "Roles",
                        principalColumn: "Id_Rol",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
