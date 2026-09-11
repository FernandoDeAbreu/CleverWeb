using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleverWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddMembroRelationToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MembroId",
                table: "Usuario",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"UPDATE ""Usuario"" SET ""MembroId"" = (SELECT MIN(""Id"") FROM ""Membro"" WHERE ""Membro"".""TenantId"" = ""Usuario"".""TenantId"") WHERE ""MembroId"" = 0;");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_MembroId",
                table: "Usuario",
                column: "MembroId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Membro_MembroId",
                table: "Usuario",
                column: "MembroId",
                principalTable: "Membro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Membro_MembroId",
                table: "Usuario");

            migrationBuilder.DropIndex(
                name: "IX_Usuario_MembroId",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "MembroId",
                table: "Usuario");
        }
    }
}
