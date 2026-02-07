using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleverWeb.Migrations
{
    /// <inheritdoc />
    public partial class AjusteTableCaixa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaldoAnterior",
                table: "Caixa");

            migrationBuilder.RenameColumn(
                name: "CaixaID",
                table: "Despesa",
                newName: "CaixaId");

            migrationBuilder.RenameColumn(
                name: "SaldoAtual",
                table: "Caixa",
                newName: "Saldo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CaixaId",
                table: "Despesa",
                newName: "CaixaID");

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
    }
}
