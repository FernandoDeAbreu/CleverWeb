using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleverWeb.Migrations
{
    /// <inheritdoc />
    public partial class MultiTenantBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contribuicao_Membro_MembroId",
                table: "Contribuicao");

            migrationBuilder.DropForeignKey(
                name: "FK_Despesa_Fornecedor_FornecedorId",
                table: "Despesa");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Usuario",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Membro",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Fornecedor",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Despesa",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Contribuicao",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Caixa",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Tenant",
                columns: new[] { "Nome", "Slug", "Ativo", "DataCriacao" },
                values: new object[] { "Tenant Padrão", "default", true, DateTime.UtcNow });

            migrationBuilder.Sql("UPDATE \"Usuario\" SET \"TenantId\" = 1 WHERE \"TenantId\" = 0;");
            migrationBuilder.Sql("UPDATE \"Membro\" SET \"TenantId\" = 1 WHERE \"TenantId\" = 0;");
            migrationBuilder.Sql("UPDATE \"Fornecedor\" SET \"TenantId\" = 1 WHERE \"TenantId\" = 0;");
            migrationBuilder.Sql("UPDATE \"Despesa\" SET \"TenantId\" = 1 WHERE \"TenantId\" = 0;");
            migrationBuilder.Sql("UPDATE \"Contribuicao\" SET \"TenantId\" = 1 WHERE \"TenantId\" = 0;");
            migrationBuilder.Sql("UPDATE \"Caixa\" SET \"TenantId\" = 1 WHERE \"TenantId\" = 0;");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_TenantId_UserName",
                table: "Usuario",
                columns: new[] { "TenantId", "UserName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Membro_TenantId",
                table: "Membro",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Fornecedor_TenantId",
                table: "Fornecedor",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Despesa_TenantId",
                table: "Despesa",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Contribuicao_TenantId",
                table: "Contribuicao",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Caixa_TenantId",
                table: "Caixa",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_Slug",
                table: "Tenant",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Caixa_Tenant_TenantId",
                table: "Caixa",
                column: "TenantId",
                principalTable: "Tenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contribuicao_Membro_MembroId",
                table: "Contribuicao",
                column: "MembroId",
                principalTable: "Membro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contribuicao_Tenant_TenantId",
                table: "Contribuicao",
                column: "TenantId",
                principalTable: "Tenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Despesa_Fornecedor_FornecedorId",
                table: "Despesa",
                column: "FornecedorId",
                principalTable: "Fornecedor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Despesa_Tenant_TenantId",
                table: "Despesa",
                column: "TenantId",
                principalTable: "Tenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Fornecedor_Tenant_TenantId",
                table: "Fornecedor",
                column: "TenantId",
                principalTable: "Tenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Membro_Tenant_TenantId",
                table: "Membro",
                column: "TenantId",
                principalTable: "Tenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Tenant_TenantId",
                table: "Usuario",
                column: "TenantId",
                principalTable: "Tenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Caixa_Tenant_TenantId",
                table: "Caixa");

            migrationBuilder.DropForeignKey(
                name: "FK_Contribuicao_Membro_MembroId",
                table: "Contribuicao");

            migrationBuilder.DropForeignKey(
                name: "FK_Contribuicao_Tenant_TenantId",
                table: "Contribuicao");

            migrationBuilder.DropForeignKey(
                name: "FK_Despesa_Fornecedor_FornecedorId",
                table: "Despesa");

            migrationBuilder.DropForeignKey(
                name: "FK_Despesa_Tenant_TenantId",
                table: "Despesa");

            migrationBuilder.DropForeignKey(
                name: "FK_Fornecedor_Tenant_TenantId",
                table: "Fornecedor");

            migrationBuilder.DropForeignKey(
                name: "FK_Membro_Tenant_TenantId",
                table: "Membro");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Tenant_TenantId",
                table: "Usuario");

            migrationBuilder.DropTable(
                name: "Tenant");

            migrationBuilder.DropIndex(
                name: "IX_Usuario_TenantId_UserName",
                table: "Usuario");

            migrationBuilder.DropIndex(
                name: "IX_Membro_TenantId",
                table: "Membro");

            migrationBuilder.DropIndex(
                name: "IX_Fornecedor_TenantId",
                table: "Fornecedor");

            migrationBuilder.DropIndex(
                name: "IX_Despesa_TenantId",
                table: "Despesa");

            migrationBuilder.DropIndex(
                name: "IX_Contribuicao_TenantId",
                table: "Contribuicao");

            migrationBuilder.DropIndex(
                name: "IX_Caixa_TenantId",
                table: "Caixa");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Membro");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Fornecedor");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Despesa");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Contribuicao");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Caixa");

            migrationBuilder.AddForeignKey(
                name: "FK_Contribuicao_Membro_MembroId",
                table: "Contribuicao",
                column: "MembroId",
                principalTable: "Membro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Despesa_Fornecedor_FornecedorId",
                table: "Despesa",
                column: "FornecedorId",
                principalTable: "Fornecedor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
