using CleverWeb.Features.Auth.ViewModel;
using CleverWeb.Features.Users.Services;
using Microsoft.AspNetCore.Mvc;

namespace CleverWeb.Features.Users
{
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (_userService.UsuarioExiste(model.UserName))
            {
                ModelState.AddModelError("", "Usuário já existe");
                return View(model);
            }

            _userService.CriarUsuario(model.UserName, model.Senha);

            return RedirectToAction("Login", "Auth");
        }
    }
}
