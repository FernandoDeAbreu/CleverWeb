using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleverWeb.Migrations
{
    /// <inheritdoc />
    public partial class UnificarUsuarioUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuario_TenantId_UserName",
                table: "Usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_TenantId",
                table: "Usuario",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_UserName",
                table: "Usuario",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuario_TenantId",
                table: "Usuario");

            migrationBuilder.DropIndex(
                name: "IX_Usuario_UserName",
                table: "Usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_TenantId_UserName",
                table: "Usuario",
                columns: new[] { "TenantId", "UserName" },
                unique: true);
        }
    }
}
