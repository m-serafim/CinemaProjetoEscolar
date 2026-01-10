using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaGestao2223226.Migrations
{
    /// <inheritdoc />
    public partial class FixRefundSystemNullableSessaoId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Sessoes_SessaoId",
                table: "Reservas");

            migrationBuilder.AlterColumn<int>(
                name: "SessaoId",
                table: "Reservas",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "FilmeTitulo",
                table: "Reservas",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SessaoDataHora",
                table: "Reservas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Sessoes_SessaoId",
                table: "Reservas",
                column: "SessaoId",
                principalTable: "Sessoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Sessoes_SessaoId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "FilmeTitulo",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "SessaoDataHora",
                table: "Reservas");

            migrationBuilder.AlterColumn<int>(
                name: "SessaoId",
                table: "Reservas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Sessoes_SessaoId",
                table: "Reservas",
                column: "SessaoId",
                principalTable: "Sessoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
