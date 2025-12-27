using System.Security.Cryptography;
using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using CleverWeb.Data;

namespace CleverWeb.Features.Users.Services
{
    public class UserService
    {
        private readonly CleverDbContext _context;

        public UserService(CleverDbContext context)
        {
            _context = context;
        }

        public bool UsuarioExiste(string username)
        {
            return _context.Usuario.Any(u => u.UserName == username);
        }

        public void CriarUsuario(string username, string senha)
        {
            var usuario = new Models.Usuario
            {
                UserName = username,
                PasswordHash = CriarHashSenha(senha)
            };

            _context.Usuario.Add(usuario);
            _context.SaveChanges();
        }

        public static string CriarHashSenha(string senha)
        {
            var salt = RandomNumberGenerator.GetBytes(16);

            var hash = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: senha,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100_000,
                    numBytesRequested: 256 / 8
                )
            );

            return $"{Convert.ToBase64String(salt)}:{hash}";
        }

    }
}
