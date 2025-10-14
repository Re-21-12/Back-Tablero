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
                    { 11, "Cliente", DateTime.UtcNow, 0, null, 0   },

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
                    // === PERMISOS PARA ROL ADMIN (ID: 1) ===
                    // Localidad
                    { 1, "Localidad:Agregar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 2, "Localidad:Editar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 3, "Localidad:Eliminar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 4, "Localidad:Consultar", DateTime.UtcNow, 1, 0, null, 0 },
                    
                    // Equipo
                    { 5, "Equipo:Agregar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 6, "Equipo:Editar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 7, "Equipo:Eliminar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 8, "Equipo:Consultar", DateTime.UtcNow, 1, 0, null, 0 },
                    
                    // Partido
                    { 9, "Partido:Agregar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 10, "Partido:Editar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 11, "Partido:Eliminar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 12, "Partido:Consultar", DateTime.UtcNow, 1, 0, null, 0 },
                    
                    // Jugador
                    { 13, "Jugador:Agregar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 14, "Jugador:Editar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 15, "Jugador:Eliminar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 16, "Jugador:Consultar", DateTime.UtcNow, 1, 0, null, 0 },
                    
                    // Cuarto
                    { 17, "Cuarto:Agregar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 18, "Cuarto:Editar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 19, "Cuarto:Eliminar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 20, "Cuarto:Consultar", DateTime.UtcNow, 1, 0, null, 0 },
                    
                    // Imagen
                    { 21, "Imagen:Agregar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 22, "Imagen:Editar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 23, "Imagen:Eliminar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 24, "Imagen:Consultar", DateTime.UtcNow, 1, 0, null, 0 },
                    
                    // Usuario
                    { 25, "Usuario:Agregar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 26, "Usuario:Editar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 27, "Usuario:Eliminar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 28, "Usuario:Consultar", DateTime.UtcNow, 1, 0, null, 0 },
                    
                    // Rol
                    { 29, "Rol:Agregar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 30, "Rol:Editar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 31, "Rol:Eliminar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 32, "Rol:Consultar", DateTime.UtcNow, 1, 0, null, 0 },
                    
                    // Permiso
                    { 33, "Permiso:Agregar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 34, "Permiso:Editar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 35, "Permiso:Eliminar", DateTime.UtcNow, 1, 0, null, 0 },
                    { 36, "Permiso:Consultar", DateTime.UtcNow, 1, 0, null, 0 },

                    // === PERMISOS PARA ROL LOCALIDAD (ID: 2) ===
                    { 37, "Localidad:Agregar", DateTime.UtcNow, 2, 0, null, 0 },
                    { 38, "Localidad:Editar", DateTime.UtcNow, 2, 0, null, 0 },
                    { 39, "Localidad:Eliminar", DateTime.UtcNow, 2, 0, null, 0 },
                    { 40, "Localidad:Consultar", DateTime.UtcNow, 2, 0, null, 0 },

                    // === PERMISOS PARA ROL EQUIPO (ID: 3) ===
                    { 41, "Equipo:Agregar", DateTime.UtcNow, 3, 0, null, 0 },
                    { 42, "Equipo:Editar", DateTime.UtcNow, 3, 0, null, 0 },
                    { 43, "Equipo:Eliminar", DateTime.UtcNow, 3, 0, null, 0 },
                    { 44, "Equipo:Consultar", DateTime.UtcNow, 3, 0, null, 0 },

                    // === PERMISOS PARA ROL PARTIDO (ID: 4) ===
                    { 45, "Partido:Agregar", DateTime.UtcNow, 4, 0, null, 0 },
                    { 46, "Partido:Editar", DateTime.UtcNow, 4, 0, null, 0 },
                    { 47, "Partido:Eliminar", DateTime.UtcNow, 4, 0, null, 0 },
                    { 48, "Partido:Consultar", DateTime.UtcNow, 4, 0, null, 0 },

                    // === PERMISOS PARA ROL JUGADOR (ID: 5) ===
                    { 49, "Jugador:Agregar", DateTime.UtcNow, 5, 0, null, 0 },
                    { 50, "Jugador:Editar", DateTime.UtcNow, 5, 0, null, 0 },
                    { 51, "Jugador:Eliminar", DateTime.UtcNow, 5, 0, null, 0 },
                    { 52, "Jugador:Consultar", DateTime.UtcNow, 5, 0, null, 0 },

                    // === PERMISOS PARA ROL CUARTO (ID: 6) ===
                    { 53, "Cuarto:Agregar", DateTime.UtcNow, 6, 0, null, 0 },
                    { 54, "Cuarto:Editar", DateTime.UtcNow, 6, 0, null, 0 },
                    { 55, "Cuarto:Eliminar", DateTime.UtcNow, 6, 0, null, 0 },
                    { 56, "Cuarto:Consultar", DateTime.UtcNow, 6, 0, null, 0 },

                    // === PERMISOS PARA ROL IMAGEN (ID: 7) ===
                    { 57, "Imagen:Agregar", DateTime.UtcNow, 7, 0, null, 0 },
                    { 58, "Imagen:Editar", DateTime.UtcNow, 7, 0, null, 0 },
                    { 59, "Imagen:Eliminar", DateTime.UtcNow, 7, 0, null, 0 },
                    { 60, "Imagen:Consultar", DateTime.UtcNow, 7, 0, null, 0 },

                    // === PERMISOS PARA ROL USUARIO (ID: 8) ===
                    { 61, "Usuario:Agregar", DateTime.UtcNow, 8, 0, null, 0 },
                    { 62, "Usuario:Editar", DateTime.UtcNow, 8, 0, null, 0 },
                    { 63, "Usuario:Eliminar", DateTime.UtcNow, 8, 0, null, 0 },
                    { 64, "Usuario:Consultar", DateTime.UtcNow, 8, 0, null, 0 },

                    // === PERMISOS PARA ROL ROL (ID: 9) ===
                    { 65, "Rol:Agregar", DateTime.UtcNow, 9, 0, null, 0 },
                    { 66, "Rol:Editar", DateTime.UtcNow, 9, 0, null, 0 },
                    { 67, "Rol:Eliminar", DateTime.UtcNow, 9, 0, null, 0 },
                    { 68, "Rol:Consultar", DateTime.UtcNow, 9, 0, null, 0 },

                    // === PERMISOS PARA ROL PERMISO (ID: 10) ===
                    { 69, "Permiso:Agregar", DateTime.UtcNow, 10, 0, null, 0 },
                    { 70, "Permiso:Editar", DateTime.UtcNow, 10, 0, null, 0 },
                    { 71, "Permiso:Eliminar", DateTime.UtcNow, 10, 0, null, 0 },
                    { 72, "Permiso:Consultar", DateTime.UtcNow, 10, 0, null, 0 },

                    // === PERMISOS PARA ROL CLIENTE (ID: 11) - Solo Consultas ===
                    { 73, "Localidad:Consultar", DateTime.UtcNow, 11, 0, null, 0 },
                    { 74, "Equipo:Consultar", DateTime.UtcNow, 11, 0, null, 0 },
                    { 75, "Partido:Consultar", DateTime.UtcNow, 11, 0, null, 0 },
                    { 76, "Jugador:Consultar", DateTime.UtcNow, 11, 0, null, 0 },
                    { 77, "Cuarto:Consultar", DateTime.UtcNow, 11, 0, null, 0 },
                    { 78, "Imagen:Consultar", DateTime.UtcNow, 11, 0, null, 0 }
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
