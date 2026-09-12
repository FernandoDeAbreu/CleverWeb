using CleverWeb.Data;
using CleverWeb.Features.Users.Services;
using CleverWeb.Features.Users.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleverWeb.Features.Users
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly CleverDbContext _db;

        public UserController(UserService userService, CleverDbContext db)
        {
            _userService = userService;
            _db = db;
        }

        private void CarregarOpcoesTenant(RegisterUserViewModel model)
        {
            var tenantAtual = HttpContext.User.FindFirst("tenant_id")?.Value;
            var tenantSelecionado = model.TenantId;

            if (User.HasClaim("is_global_admin", "true"))
            {
                model.TenantOptions = _db.Tenant
                    .Where(x => x.Ativo)
                    .OrderBy(x => x.Nome)
                    .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Nome,
                        Selected = x.Id == tenantSelecionado
                    })
                    .ToList();

                if (!model.TenantOptions.Any(x => x.Value == model.TenantId.ToString()))
                    model.TenantId = string.IsNullOrWhiteSpace(tenantAtual) ? 0 : int.Parse(tenantAtual);
            }
            else
            {
                model.TenantId = string.IsNullOrWhiteSpace(tenantAtual) ? 0 : int.Parse(tenantAtual);
                model.TenantOptions = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
                {
                    new() { Value = model.TenantId.ToString(), Text = _db.Tenant.FirstOrDefault(x => x.Id == model.TenantId)?.Nome ?? "Tenant atual" }
                };
            }

            model.MembroOptions = _db.Membro
                .Where(x => x.TenantId == model.TenantId)
                .OrderBy(x => x.Nome)
                .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Nome,
                    Selected = x.Id == model.MembroId
                })
                .ToList();
        }

        private void CarregarOpcoesTenant(EditUserViewModel model)
        {
            var tenantAtual = HttpContext.User.FindFirst("tenant_id")?.Value;
            var tenantSelecionado = model.TenantId;

            if (User.HasClaim("is_global_admin", "true"))
            {
                model.TenantOptions = _db.Tenant
                    .Where(x => x.Ativo)
                    .OrderBy(x => x.Nome)
                    .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Nome,
                        Selected = x.Id == tenantSelecionado
                    })
                    .ToList();

                if (!model.TenantOptions.Any(x => x.Value == model.TenantId.ToString()))
                    model.TenantId = string.IsNullOrWhiteSpace(tenantAtual) ? 0 : int.Parse(tenantAtual);
            }
            else
            {
                model.TenantId = string.IsNullOrWhiteSpace(tenantAtual) ? 0 : int.Parse(tenantAtual);
                model.TenantOptions = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
                {
                    new() { Value = model.TenantId.ToString(), Text = _db.Tenant.FirstOrDefault(x => x.Id == model.TenantId)?.Nome ?? "Tenant atual" }
                };
            }

            model.MembroOptions = _db.Membro
                .Where(x => x.TenantId == model.TenantId)
                .OrderBy(x => x.Nome)
                .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Nome,
                    Selected = x.Id == model.MembroId
                })
                .ToList();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var usuarios = _userService.ObterUsuarios();
            return View(usuarios);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterUserViewModel();
            var tenantAtual = HttpContext.User.FindFirst("tenant_id")?.Value;
            if (!string.IsNullOrWhiteSpace(tenantAtual) && int.TryParse(tenantAtual, out var id))
                model.TenantId = id;

            CarregarOpcoesTenant(model);
            return View(model);
        }

        [HttpGet]
        public IActionResult MembrosPorTenant(int tenantId)
        {
            if (!User.HasClaim("is_global_admin", "true"))
            {
                var tenantAtual = HttpContext.User.FindFirst("tenant_id")?.Value;
                if (!int.TryParse(tenantAtual, out var tenantIdAtual) || tenantIdAtual != tenantId)
                    return Forbid();
            }

            if (!_db.Tenant.Any(tenant => tenant.Id == tenantId && tenant.Ativo))
                return BadRequest();

            var membros = _db.Membro
                .Where(membro => membro.TenantId == tenantId)
                .OrderBy(membro => membro.Nome)
                .Select(membro => new
                {
                    id = membro.Id,
                    nome = membro.Nome
                })
                .ToList();

            return Json(membros);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterUserViewModel model)
        {
            if (!User.HasClaim("is_global_admin", "true"))
            {
                var tenantAtual = HttpContext.User.FindFirst("tenant_id")?.Value;
                if (string.IsNullOrWhiteSpace(tenantAtual) || !int.TryParse(tenantAtual, out var tenantIdAtual))
                {
                    ModelState.AddModelError(string.Empty, "Tenant do usuário não encontrado para o cadastro atual.");
                    return View(model);
                }

                model.TenantId = tenantIdAtual;
            }
            else
            {
                CarregarOpcoesTenant(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            if (User.HasClaim("is_global_admin", "true") && model.TenantId <= 0)
            {
                ModelState.AddModelError(nameof(model.TenantId), "Selecione a igreja/tenant do usuário.");
                return View(model);
            }

            if (model.MembroId <= 0)
            {
                ModelState.AddModelError(nameof(model.MembroId), "Selecione o membro relacionado a este usuário.");
                return View(model);
            }

            if (!_db.Membro.Any(x => x.Id == model.MembroId && x.TenantId == model.TenantId))
            {
                ModelState.AddModelError(nameof(model.MembroId), "O membro selecionado não pertence ao tenant informado.");
                return View(model);
            }

            if (_userService.UsuarioExiste(model.UserName))
            {
                ModelState.AddModelError(string.Empty, "Já existe um usuário com este nome no banco.");
                return View(model);
            }

            if (!_db.Tenant.Any(x => x.Id == model.TenantId && x.Ativo))
            {
                ModelState.AddModelError(nameof(model.TenantId), "Essa igreja/tenant não está ativa ou não existe.");
                return View(model);
            }

            _userService.CriarUsuario(model.UserName, model.Senha, model.TenantId, model.MembroId);

            TempData["Success"] = "Usuário cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!User.HasClaim("is_global_admin", "true"))
                return Forbid();

            var usuario = _userService.ObterUsuario(id);
            if (usuario == null)
                return NotFound();

            var model = new EditUserViewModel
            {
                Id = usuario.Id,
                UserName = usuario.UserName,
                TenantId = usuario.TenantId,
                MembroId = usuario.MembroId,
                Ativo = usuario.Ativo,
                IsGlobalAdmin = usuario.IsGlobalAdmin
            };

            CarregarOpcoesTenant(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, EditUserViewModel model)
        {
            if (!User.HasClaim("is_global_admin", "true"))
                return Forbid();

            var usuarioAtualId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioAtualId != null && int.TryParse(usuarioAtualId, out var idAtual)
                && idAtual == id && !model.IsGlobalAdmin)
            {
                ModelState.AddModelError(string.Empty, "Você não pode remover o status de administrador global do seu próprio usuário.");
                CarregarOpcoesTenant(model);
                return View(model);
            }

            CarregarOpcoesTenant(model);

            if (!ModelState.IsValid)
                return View(model);

            if (model.MembroId <= 0)
            {
                ModelState.AddModelError(nameof(model.MembroId), "Selecione o membro relacionado a este usuário.");
                return View(model);
            }

            if (!_db.Membro.Any(x => x.Id == model.MembroId && x.TenantId == model.TenantId))
            {
                ModelState.AddModelError(nameof(model.MembroId), "O membro selecionado não pertence ao tenant informado.");
                return View(model);
            }

            if (!_userService.AtualizarUsuario(id, model.UserName, model.Ativo, model.IsGlobalAdmin, model.TenantId, model.MembroId, model.NovaSenha))
            {
                ModelState.AddModelError(string.Empty, "Não foi possível atualizar este usuário. Verifique o tenant, o membro e a unicidade do nome de usuário.");
                return View(model);
            }

            TempData["Success"] = "Usuário atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (!User.HasClaim("is_global_admin", "true"))
                return Forbid();

            if (!_userService.ExcluirUsuario(id))
            {
                TempData["Success"] = "Usuário não encontrado ou não pode ser removido.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Usuário removido com sucesso.";
            return RedirectToAction(nameof(Index));
        }
    }
}
