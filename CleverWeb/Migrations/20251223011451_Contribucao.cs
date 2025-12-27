using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleverWeb.Migrations
{
    /// <inheritdoc />
    public partial class Contribucao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataCadastro",
                table: "Contribuicao",
                newName: "DataPagamento");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataLancamanto",
                table: "Contribuicao",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MembroId",
                table: "Contribuicao",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Valor",
                table: "Contribuicao",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataLancamanto",
                table: "Contribuicao");

            migrationBuilder.DropColumn(
                name: "MembroId",
                table: "Contribuicao");

            migrationBuilder.DropColumn(
                name: "Valor",
                table: "Contribuicao");

            migrationBuilder.RenameColumn(
                name: "DataPagamento",
                table: "Contribuicao",
                newName: "DataCadastro");
        }
    }
}
