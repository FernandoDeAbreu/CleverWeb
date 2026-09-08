using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleverWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddIsGlobalAdminToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGlobalAdmin",
                table: "Usuario",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGlobalAdmin",
                table: "Usuario");
        }
    }
}
