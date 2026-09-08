using System.Security.Cryptography;
using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using CleverWeb.Data;
using CleverWeb.Features.Auth.Services;
using CleverWeb.Infrastructure.Tenant;

namespace CleverWeb.Features.Users.Services
{
    public class UserService
    {
        private readonly CleverDbContext _context;
        private readonly ITenantAccessor _tenantAccessor;

        public UserService(CleverDbContext context, ITenantAccessor tenantAccessor)
        {
            _context = context;
            _tenantAccessor = tenantAccessor;
        }

        public bool UsuarioExiste(string username)
        {
            var tenantId = _tenantAccessor.CurrentTenantId ?? 1;
            return _context.Usuario.Any(u => u.UserName == username && u.TenantId == tenantId);
        }

        public void CriarUsuario(string username, string senha, int? tenantId = null)
        {
            var tenantAtual = tenantId ?? _tenantAccessor.CurrentTenantId ?? 1;
            var usuario = new Models.Usuario
            {
                TenantId = tenantAtual,
                UserName = username,
                PasswordHash = CriarHashSenha(senha)
            };

            _context.Usuario.Add(usuario);
            _context.SaveChanges();
        }

        public bool AtualizarSenha(string username, string novaSenha, int? tenantId = null)
        {
            var tenantAtual = tenantId ?? _tenantAccessor.CurrentTenantId ?? 1;
            var usuario = _context.Usuario.FirstOrDefault(u => u.UserName == username && u.TenantId == tenantAtual);

            if (usuario == null)
                return false;

            usuario.PasswordHash = CriarHashSenha(novaSenha);
            _context.SaveChanges();
            return true;
        }

        public static string CriarHashSenha(string senha)
        {
            return AuthService.HashSenha(senha);
        }

    }
}
