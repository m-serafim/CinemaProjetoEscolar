using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaGestao2223226.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatSelectionAndFilmeStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ColunasSala",
                table: "Sessoes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FilasSala",
                table: "Sessoes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LugaresSelecionados",
                table: "Reservas",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Filmes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColunasSala",
                table: "Sessoes");

            migrationBuilder.DropColumn(
                name: "FilasSala",
                table: "Sessoes");

            migrationBuilder.DropColumn(
                name: "LugaresSelecionados",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Filmes");
        }
    }
}
