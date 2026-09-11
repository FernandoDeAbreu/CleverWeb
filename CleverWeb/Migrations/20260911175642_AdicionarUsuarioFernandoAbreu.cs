using CleverWeb.Features.Auth.Services;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleverWeb.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarUsuarioFernandoAbreu : Migration
    {
        private const string UserName = "fernando.abreu";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var passwordHash = AuthService.HashSenha("temp123");

            migrationBuilder.Sql($@"
INSERT INTO ""Usuario"" (""TenantId"", ""MembroId"", ""UserName"", ""PasswordHash"", ""Ativo"", ""IsGlobalAdmin"")
SELECT m.""TenantId"", m.""Id"", '{UserName}', '{passwordHash}', 1, 1
FROM ""Membro"" m
WHERE m.""Email"" = 'fernando.abreu@email.com'
  AND NOT EXISTS (SELECT 1 FROM ""Usuario"" u WHERE u.""UserName"" = '{UserName}')
LIMIT 1;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"DELETE FROM ""Usuario"" WHERE ""UserName"" = '{UserName}';");
        }
    }
}
