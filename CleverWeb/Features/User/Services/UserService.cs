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
            return _context.Usuario.Any(u => u.UserName == username);
        }

        public bool UsuarioExiste(string username, int tenantId)
        {
            return _context.Usuario.Any(u => u.UserName == username);
        }

        public List<Models.Usuario> ObterUsuarios(int? tenantId = null)
        {
            var tenantAtual = tenantId ?? _tenantAccessor.CurrentTenantId ?? 1;
            return _context.Usuario
                .Where(u => u.TenantId == tenantAtual)
                .OrderBy(u => u.UserName)
                .ToList();
        }

        public Models.Usuario? ObterUsuario(int id, int? tenantId = null)
        {
            var tenantAtual = tenantId ?? _tenantAccessor.CurrentTenantId ?? 1;
            return _context.Usuario.FirstOrDefault(u => u.Id == id && u.TenantId == tenantAtual);
        }

        public void CriarUsuario(string username, string senha, int? tenantId = null, int? membroId = null, bool isGlobalAdmin = false, bool ativo = true)
        {
            var tenantAtual = tenantId ?? _tenantAccessor.CurrentTenantId ?? 1;
            var membroAtual = membroId ?? _context.Membro.FirstOrDefault(m => m.TenantId == tenantAtual)?.Id ?? 0;

            if (membroAtual <= 0)
                throw new InvalidOperationException("É obrigatório selecionar um membro válido para o usuário.");

            var usuario = new Models.Usuario
            {
                TenantId = tenantAtual,
                MembroId = membroAtual,
                UserName = username,
                PasswordHash = CriarHashSenha(senha),
                IsGlobalAdmin = isGlobalAdmin,
                Ativo = ativo
            };

            _context.Usuario.Add(usuario);
            _context.SaveChanges();
        }

        public bool AtualizarSenha(string username, string novaSenha, int? tenantId = null)
        {
            var tenantAtual = tenantId ?? _tenantAccessor.CurrentTenantId ?? 1;
            var usuario = _context.Usuario.FirstOrDefault(u => u.UserName == username);

            if (usuario == null)
                return false;

            usuario.PasswordHash = CriarHashSenha(novaSenha);
            _context.SaveChanges();
            return true;
        }

        public bool AtualizarUsuario(int id, string username, bool ativo, bool isGlobalAdmin, int? tenantId = null, int? membroId = null, string? novaSenha = null)
        {
            var tenantAtual = tenantId ?? _tenantAccessor.CurrentTenantId ?? 1;
            var usuario = _context.Usuario.FirstOrDefault(u => u.Id == id);
            if (usuario == null)
                return false;

            var tenantDestino = tenantAtual;
            var membroDestino = membroId ?? usuario.MembroId;

            if (membroDestino <= 0 || !_context.Membro.Any(m => m.Id == membroDestino && m.TenantId == tenantDestino))
                return false;

            if (_context.Usuario.Any(u => u.UserName == username && u.Id != id))
                return false;

            usuario.UserName = username;
            usuario.Ativo = ativo;
            usuario.IsGlobalAdmin = isGlobalAdmin;
            usuario.TenantId = tenantDestino;
            usuario.MembroId = membroDestino;

            if (!string.IsNullOrWhiteSpace(novaSenha))
                usuario.PasswordHash = CriarHashSenha(novaSenha);

            _context.SaveChanges();
            return true;
        }

        public bool ExcluirUsuario(int id, int? tenantId = null)
        {
            var tenantAtual = tenantId ?? _tenantAccessor.CurrentTenantId ?? 1;
            var usuario = _context.Usuario.FirstOrDefault(u => u.Id == id && u.TenantId == tenantAtual);
            if (usuario == null)
                return false;

            _context.Usuario.Remove(usuario);
            _context.SaveChanges();
            return true;
        }

        public static string CriarHashSenha(string senha)
        {
            return AuthService.HashSenha(senha);
        }

    }
}
