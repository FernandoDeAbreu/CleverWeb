using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleverWeb.Migrations
{
    /// <inheritdoc />
    public partial class Caixa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CaixaID",
                table: "Despesa",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CaixaID",
                table: "Contribuicao",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Caixa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DtFechamento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SaldoAnterior = table.Column<decimal>(type: "TEXT", nullable: false),
                    SaldoAtual = table.Column<decimal>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caixa", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Caixa");

            migrationBuilder.DropColumn(
                name: "CaixaID",
                table: "Despesa");

            migrationBuilder.DropColumn(
                name: "CaixaID",
                table: "Contribuicao");
        }
    }
}
