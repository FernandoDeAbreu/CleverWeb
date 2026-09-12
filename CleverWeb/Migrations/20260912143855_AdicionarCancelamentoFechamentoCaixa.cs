using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleverWeb.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCancelamentoFechamentoCaixa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GeradaNoFechamento",
                table: "Despesa",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Cancelado",
                table: "Caixa",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DtCancelamento",
                table: "Caixa",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoCancelamento",
                table: "Caixa",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioCancelamentoId",
                table: "Caixa",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeradaNoFechamento",
                table: "Despesa");

            migrationBuilder.DropColumn(
                name: "Cancelado",
                table: "Caixa");

            migrationBuilder.DropColumn(
                name: "DtCancelamento",
                table: "Caixa");

            migrationBuilder.DropColumn(
                name: "MotivoCancelamento",
                table: "Caixa");

            migrationBuilder.DropColumn(
                name: "UsuarioCancelamentoId",
                table: "Caixa");
        }
    }
}
