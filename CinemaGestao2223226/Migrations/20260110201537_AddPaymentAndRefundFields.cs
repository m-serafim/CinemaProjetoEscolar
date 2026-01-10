using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaGestao2223226.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentAndRefundFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AvisoVisualizado",
                table: "Reservas",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CVV",
                table: "Reservas",
                type: "TEXT",
                maxLength: 4,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CanceladaPeloSistema",
                table: "Reservas",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataReembolso",
                table: "Reservas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoCancelamento",
                table: "Reservas",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeCartao",
                table: "Reservas",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroCartao",
                table: "Reservas",
                type: "TEXT",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Reembolsado",
                table: "Reservas",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ValidadeCartao",
                table: "Reservas",
                type: "TEXT",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorPago",
                table: "Reservas",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvisoVisualizado",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "CVV",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "CanceladaPeloSistema",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "DataReembolso",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "MotivoCancelamento",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "NomeCartao",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "NumeroCartao",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "Reembolsado",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "ValidadeCartao",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "ValorPago",
                table: "Reservas");
        }
    }
}
