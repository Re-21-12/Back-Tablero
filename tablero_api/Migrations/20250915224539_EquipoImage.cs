using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tablero_api.Migrations
{
    /// <inheritdoc />
    public partial class EquipoImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "url_imagen",
                table: "Equipos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "url_imagen",
                table: "Equipos");
        }
    }
}
