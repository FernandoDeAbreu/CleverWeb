using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleverWeb.Migrations
{
    /// <inheritdoc />
    public partial class InserirCampoCAIXA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Saldo",
                table: "Caixa",
                newName: "SaldoAtual");

            migrationBuilder.AddColumn<decimal>(
                name: "SaldoAnterior",
                table: "Caixa",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaldoAnterior",
                table: "Caixa");

            migrationBuilder.RenameColumn(
                name: "SaldoAtual",
                table: "Caixa",
                newName: "Saldo");
        }
    }
}
