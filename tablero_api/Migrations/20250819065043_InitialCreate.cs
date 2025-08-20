using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tablero_api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Imagenes",
                columns: table => new
                {
                    id_Imagen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imagenes", x => x.id_Imagen);
                });

            migrationBuilder.CreateTable(
                name: "Localidades",
                columns: table => new
                {
                    id_Localidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localidades", x => x.id_Localidad);
                });

            migrationBuilder.CreateTable(
                name: "Equipos",
                columns: table => new
                {
                    id_Equipo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    id_Localidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipos", x => x.id_Equipo);
                    table.ForeignKey(
                        name: "FK_Equipos_Localidades_id_Localidad",
                        column: x => x.id_Localidad,
                        principalTable: "Localidades",
                        principalColumn: "id_Localidad",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Partidos",
                columns: table => new
                {
                    id_Partido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_Localidad = table.Column<int>(type: "int", nullable: false),
                    id_Local = table.Column<int>(type: "int", nullable: false),
                    id_Visitante = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidos", x => x.id_Partido);
                    table.ForeignKey(
                        name: "FK_Partidos_Equipos_id_Local",
                        column: x => x.id_Local,
                        principalTable: "Equipos",
                        principalColumn: "id_Equipo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partidos_Equipos_id_Visitante",
                        column: x => x.id_Visitante,
                        principalTable: "Equipos",
                        principalColumn: "id_Equipo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partidos_Localidades_id_Localidad",
                        column: x => x.id_Localidad,
                        principalTable: "Localidades",
                        principalColumn: "id_Localidad",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cuartos",
                columns: table => new
                {
                    id_Cuarto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    No_Cuarto = table.Column<int>(type: "int", nullable: false),
                    duenio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Total_Punteo = table.Column<int>(type: "int", nullable: false),
                    Total_Faltas = table.Column<int>(type: "int", nullable: false),
                    id_Partido = table.Column<int>(type: "int", nullable: false),
                    id_Equipo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuartos", x => x.id_Cuarto);
                    table.ForeignKey(
                        name: "FK_Cuartos_Equipos_id_Equipo",
                        column: x => x.id_Equipo,
                        principalTable: "Equipos",
                        principalColumn: "id_Equipo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cuartos_Partidos_id_Partido",
                        column: x => x.id_Partido,
                        principalTable: "Partidos",
                        principalColumn: "id_Partido",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cuartos_id_Equipo",
                table: "Cuartos",
                column: "id_Equipo");

            migrationBuilder.CreateIndex(
                name: "IX_Cuartos_id_Partido",
                table: "Cuartos",
                column: "id_Partido");

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_id_Localidad",
                table: "Equipos",
                column: "id_Localidad");

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_id_Local",
                table: "Partidos",
                column: "id_Local");

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_id_Localidad",
                table: "Partidos",
                column: "id_Localidad");

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_id_Visitante",
                table: "Partidos",
                column: "id_Visitante");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cuartos");

            migrationBuilder.DropTable(
                name: "Imagenes");

            migrationBuilder.DropTable(
                name: "Partidos");

            migrationBuilder.DropTable(
                name: "Equipos");

            migrationBuilder.DropTable(
                name: "Localidades");
        }
    }
}
