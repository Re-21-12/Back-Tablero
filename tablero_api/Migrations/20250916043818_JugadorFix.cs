using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tablero_api.Migrations
{
    /// <inheritdoc />
    public partial class JugadorFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "estatura",
                table: "Jugadores",
                newName: "Estatura");

            migrationBuilder.AddColumn<string>(
                name: "Nacionalidad",
                table: "Jugadores",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nacionalidad",
                table: "Jugadores");

            migrationBuilder.RenameColumn(
                name: "Estatura",
                table: "Jugadores",
                newName: "estatura");
        }
    }
}
