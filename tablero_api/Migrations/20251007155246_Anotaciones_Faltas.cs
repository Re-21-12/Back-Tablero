using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tablero_api.Migrations
{
    /// <inheritdoc />
    public partial class Anotaciones_Faltas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Anotaciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    total_anotaciones = table.Column<int>(type: "int", nullable: false),
                    id_jugador = table.Column<int>(type: "int", nullable: false),
                    jugadorid_Jugador = table.Column<int>(type: "int", nullable: false),
                    id_cuarto = table.Column<int>(type: "int", nullable: false),
                    cuartoid_Cuarto = table.Column<int>(type: "int", nullable: false),
                    id_partido = table.Column<int>(type: "int", nullable: false),
                    partidoid_Partido = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anotaciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_Anotaciones_Cuartos_cuartoid_Cuarto",
                        column: x => x.cuartoid_Cuarto,
                        principalTable: "Cuartos",
                        principalColumn: "id_Cuarto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Anotaciones_Jugadores_jugadorid_Jugador",
                        column: x => x.jugadorid_Jugador,
                        principalTable: "Jugadores",
                        principalColumn: "id_Jugador",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Anotaciones_Partidos_partidoid_Partido",
                        column: x => x.partidoid_Partido,
                        principalTable: "Partidos",
                        principalColumn: "id_Partido",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Faltas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    total_falta = table.Column<int>(type: "int", nullable: false),
                    id_jugador = table.Column<int>(type: "int", nullable: false),
                    jugadorid_Jugador = table.Column<int>(type: "int", nullable: false),
                    id_cuarto = table.Column<int>(type: "int", nullable: false),
                    cuartoid_Cuarto = table.Column<int>(type: "int", nullable: false),
                    id_partido = table.Column<int>(type: "int", nullable: false),
                    partidoid_Partido = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faltas", x => x.id);
                    table.ForeignKey(
                        name: "FK_Faltas_Cuartos_cuartoid_Cuarto",
                        column: x => x.cuartoid_Cuarto,
                        principalTable: "Cuartos",
                        principalColumn: "id_Cuarto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Faltas_Jugadores_jugadorid_Jugador",
                        column: x => x.jugadorid_Jugador,
                        principalTable: "Jugadores",
                        principalColumn: "id_Jugador",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Faltas_Partidos_partidoid_Partido",
                        column: x => x.partidoid_Partido,
                        principalTable: "Partidos",
                        principalColumn: "id_Partido",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Anotaciones_cuartoid_Cuarto",
                table: "Anotaciones",
                column: "cuartoid_Cuarto");

            migrationBuilder.CreateIndex(
                name: "IX_Anotaciones_jugadorid_Jugador",
                table: "Anotaciones",
                column: "jugadorid_Jugador");

            migrationBuilder.CreateIndex(
                name: "IX_Anotaciones_partidoid_Partido",
                table: "Anotaciones",
                column: "partidoid_Partido");

            migrationBuilder.CreateIndex(
                name: "IX_Faltas_cuartoid_Cuarto",
                table: "Faltas",
                column: "cuartoid_Cuarto");

            migrationBuilder.CreateIndex(
                name: "IX_Faltas_jugadorid_Jugador",
                table: "Faltas",
                column: "jugadorid_Jugador");

            migrationBuilder.CreateIndex(
                name: "IX_Faltas_partidoid_Partido",
                table: "Faltas",
                column: "partidoid_Partido");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Anotaciones");

            migrationBuilder.DropTable(
                name: "Faltas");
        }
    }
}
