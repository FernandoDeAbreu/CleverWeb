using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleverWeb.Migrations
{
    /// <inheritdoc />
    public partial class AjusteDeSaldos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

                      Delete from Caixa where ID < 5;
                      Update Caixa set SaldoAtual = '2808.25' Where Id = 5;
                      Update Caixa set SaldoAtual = '1047.50' Where Id = 6;
                      Update Caixa set SaldoAtual = '0.0' Where Id = 7;
           ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}