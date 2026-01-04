using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleverWeb.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarFornecedores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Fornecedor",
                column: "Descricao",
                values: new object[]
                {
                    "Equatorial Energia",
                    "Caema - Companhia de agua",
                    "Monitoramento - Alarme",
                    "Zeladoria",
                    "Deposito de agua mineral",
                    "Desp. Santa Ceia",
                    "Material de limpeza",
                    "Outros"
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}