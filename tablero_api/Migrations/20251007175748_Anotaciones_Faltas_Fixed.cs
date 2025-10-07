using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tablero_api.Migrations
{
    /// <inheritdoc />
    public partial class Anotaciones_Faltas_Fixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Anotaciones_Cuartos_cuartoid_Cuarto",
                table: "Anotaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Anotaciones_Jugadores_jugadorid_Jugador",
                table: "Anotaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Anotaciones_Partidos_partidoid_Partido",
                table: "Anotaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Faltas_Cuartos_cuartoid_Cuarto",
                table: "Faltas");

            migrationBuilder.DropForeignKey(
                name: "FK_Faltas_Jugadores_jugadorid_Jugador",
                table: "Faltas");

            migrationBuilder.DropForeignKey(
                name: "FK_Faltas_Partidos_partidoid_Partido",
                table: "Faltas");

            migrationBuilder.DropIndex(
                name: "IX_Faltas_cuartoid_Cuarto",
                table: "Faltas");

            migrationBuilder.DropIndex(
                name: "IX_Faltas_jugadorid_Jugador",
                table: "Faltas");

            migrationBuilder.DropIndex(
                name: "IX_Faltas_partidoid_Partido",
                table: "Faltas");

            migrationBuilder.DropIndex(
                name: "IX_Anotaciones_cuartoid_Cuarto",
                table: "Anotaciones");

            migrationBuilder.DropIndex(
                name: "IX_Anotaciones_jugadorid_Jugador",
                table: "Anotaciones");

            migrationBuilder.DropIndex(
                name: "IX_Anotaciones_partidoid_Partido",
                table: "Anotaciones");

            migrationBuilder.DropColumn(
                name: "cuartoid_Cuarto",
                table: "Faltas");

            migrationBuilder.DropColumn(
                name: "jugadorid_Jugador",
                table: "Faltas");

            migrationBuilder.DropColumn(
                name: "partidoid_Partido",
                table: "Faltas");

            migrationBuilder.DropColumn(
                name: "cuartoid_Cuarto",
                table: "Anotaciones");

            migrationBuilder.DropColumn(
                name: "jugadorid_Jugador",
                table: "Anotaciones");

            migrationBuilder.DropColumn(
                name: "partidoid_Partido",
                table: "Anotaciones");

            migrationBuilder.CreateIndex(
                name: "IX_Faltas_id_cuarto",
                table: "Faltas",
                column: "id_cuarto");

            migrationBuilder.CreateIndex(
                name: "IX_Faltas_id_jugador",
                table: "Faltas",
                column: "id_jugador");

            migrationBuilder.CreateIndex(
                name: "IX_Faltas_id_partido",
                table: "Faltas",
                column: "id_partido");

            migrationBuilder.CreateIndex(
                name: "IX_Anotaciones_id_cuarto",
                table: "Anotaciones",
                column: "id_cuarto");

            migrationBuilder.CreateIndex(
                name: "IX_Anotaciones_id_jugador",
                table: "Anotaciones",
                column: "id_jugador");

            migrationBuilder.CreateIndex(
                name: "IX_Anotaciones_id_partido",
                table: "Anotaciones",
                column: "id_partido");

            migrationBuilder.AddForeignKey(
                name: "FK_Anotaciones_Cuartos_id_cuarto",
                table: "Anotaciones",
                column: "id_cuarto",
                principalTable: "Cuartos",
                principalColumn: "id_Cuarto",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Anotaciones_Jugadores_id_jugador",
                table: "Anotaciones",
                column: "id_jugador",
                principalTable: "Jugadores",
                principalColumn: "id_Jugador",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Anotaciones_Partidos_id_partido",
                table: "Anotaciones",
                column: "id_partido",
                principalTable: "Partidos",
                principalColumn: "id_Partido",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Faltas_Cuartos_id_cuarto",
                table: "Faltas",
                column: "id_cuarto",
                principalTable: "Cuartos",
                principalColumn: "id_Cuarto",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Faltas_Jugadores_id_jugador",
                table: "Faltas",
                column: "id_jugador",
                principalTable: "Jugadores",
                principalColumn: "id_Jugador",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Faltas_Partidos_id_partido",
                table: "Faltas",
                column: "id_partido",
                principalTable: "Partidos",
                principalColumn: "id_Partido",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Anotaciones_Cuartos_id_cuarto",
                table: "Anotaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Anotaciones_Jugadores_id_jugador",
                table: "Anotaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Anotaciones_Partidos_id_partido",
                table: "Anotaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Faltas_Cuartos_id_cuarto",
                table: "Faltas");

            migrationBuilder.DropForeignKey(
                name: "FK_Faltas_Jugadores_id_jugador",
                table: "Faltas");

            migrationBuilder.DropForeignKey(
                name: "FK_Faltas_Partidos_id_partido",
                table: "Faltas");

            migrationBuilder.DropIndex(
                name: "IX_Faltas_id_cuarto",
                table: "Faltas");

            migrationBuilder.DropIndex(
                name: "IX_Faltas_id_jugador",
                table: "Faltas");

            migrationBuilder.DropIndex(
                name: "IX_Faltas_id_partido",
                table: "Faltas");

            migrationBuilder.DropIndex(
                name: "IX_Anotaciones_id_cuarto",
                table: "Anotaciones");

            migrationBuilder.DropIndex(
                name: "IX_Anotaciones_id_jugador",
                table: "Anotaciones");

            migrationBuilder.DropIndex(
                name: "IX_Anotaciones_id_partido",
                table: "Anotaciones");

            migrationBuilder.AddColumn<int>(
                name: "cuartoid_Cuarto",
                table: "Faltas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "jugadorid_Jugador",
                table: "Faltas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "partidoid_Partido",
                table: "Faltas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "cuartoid_Cuarto",
                table: "Anotaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "jugadorid_Jugador",
                table: "Anotaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "partidoid_Partido",
                table: "Anotaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Anotaciones_Cuartos_cuartoid_Cuarto",
                table: "Anotaciones",
                column: "cuartoid_Cuarto",
                principalTable: "Cuartos",
                principalColumn: "id_Cuarto",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Anotaciones_Jugadores_jugadorid_Jugador",
                table: "Anotaciones",
                column: "jugadorid_Jugador",
                principalTable: "Jugadores",
                principalColumn: "id_Jugador",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Anotaciones_Partidos_partidoid_Partido",
                table: "Anotaciones",
                column: "partidoid_Partido",
                principalTable: "Partidos",
                principalColumn: "id_Partido",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Faltas_Cuartos_cuartoid_Cuarto",
                table: "Faltas",
                column: "cuartoid_Cuarto",
                principalTable: "Cuartos",
                principalColumn: "id_Cuarto",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Faltas_Jugadores_jugadorid_Jugador",
                table: "Faltas",
                column: "jugadorid_Jugador",
                principalTable: "Jugadores",
                principalColumn: "id_Jugador",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Faltas_Partidos_partidoid_Partido",
                table: "Faltas",
                column: "partidoid_Partido",
                principalTable: "Partidos",
                principalColumn: "id_Partido",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
