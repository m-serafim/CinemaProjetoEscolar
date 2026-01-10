using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaGestao2223226.Migrations
{
    /// <inheritdoc />
    public partial class AddBannerAndMovieDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BannerUrl",
                table: "Filmes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassificacaoEtaria",
                table: "Filmes",
                type: "TEXT",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataEstreia",
                table: "Filmes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DestaqueHome",
                table: "Filmes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Elenco",
                table: "Filmes",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Realizador",
                table: "Filmes",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrailerUrl",
                table: "Filmes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerUrl",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "ClassificacaoEtaria",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "DataEstreia",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "DestaqueHome",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "Elenco",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "Realizador",
                table: "Filmes");

            migrationBuilder.DropColumn(
                name: "TrailerUrl",
                table: "Filmes");
        }
    }
}
