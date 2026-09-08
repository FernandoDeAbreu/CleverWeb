using CleverWeb.Data;
using CleverWeb.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace CleverWeb.Features.Auth.Services
{
    public class AuthService
    {
        private readonly CleverDbContext _db;

        public AuthService(CleverDbContext db)
        {
            _db = db;
        }

        public Usuario? Autenticar(string username, string senha, int? tenantId = null)
        {
            var query = _db.Usuario.AsQueryable();

            if (tenantId.HasValue)
                query = query.Where(u => u.TenantId == tenantId.Value);

            var user = query.FirstOrDefault(u => u.UserName == username && u.Ativo);
            if (user == null) return null;

            var verificarSenha = VerificarSenha(senha, user.PasswordHash) ? user : null;
            return verificarSenha;
        }

        public List<CleverWeb.Models.Tenant> ObterTenantsAtivos()
        {
            return _db.Tenant.Where(x => x.Ativo).OrderBy(x => x.Nome).ToList();
        }

        public string ObterSlugTenant(int tenantId)
        {
            var tenant = _db.Tenant.FirstOrDefault(x => x.Id == tenantId && x.Ativo);
            return tenant?.Slug ?? "default";
        }

        public static string HashSenha(string senha)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: senha,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100_000,
                numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}:{hash}";
        }

        public static bool VerificarSenha(string senha, string hashArmazenado)
        {
            if (string.IsNullOrWhiteSpace(hashArmazenado))
                return false;

            var separator = hashArmazenado.Contains(':') ? ':' : (hashArmazenado.Contains('.') ? '.' : (char?)null);
            if (separator is null)
                return false;

            var parts = hashArmazenado.Split(separator.Value);
            if (parts.Length != 2)
                return false;

            var salt = Convert.FromBase64String(parts[0]);
            var hashOriginal = parts[1];

            var hashTentativa = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: senha,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100_000,
                    numBytesRequested: 256 / 8
                )
            );

            return hashOriginal == hashTentativa;
        }



    }
}