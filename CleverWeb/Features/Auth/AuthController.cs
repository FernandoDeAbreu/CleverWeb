using CleverWeb.Features.Auth.Services;
using CleverWeb.Features.Auth.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleverWeb.Features.Auth
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("/login")]
        public IActionResult Login()
        {
            ViewBag.Tenants = _authService.ObterTenantsAtivos()
                .Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Nome
                }).ToList();

            return View(new LoginViewModel { TenantId = _authService.ObterTenantsAtivos().FirstOrDefault()?.Id });
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewBag.Tenants = _authService.ObterTenantsAtivos()
                .Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Nome
                }).ToList();

            if (!ModelState.IsValid) return View(model);

            var usuario = _authService.Autenticar(model.UserName, model.Senha, model.TenantId);
            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuário, empresa ou senha inválidos");
                return View(model);
            }

            var tenantNome = _authService.ObterTenantsAtivos()
                .FirstOrDefault(t => t.Id == usuario.TenantId)?.Nome ?? "Empresa";

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.UserName),
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim("tenant_id", usuario.TenantId.ToString()),
            new Claim("tenant_slug", _authService.ObterSlugTenant(usuario.TenantId)),
            new Claim("tenant_name", tenantNome)
        };

            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
