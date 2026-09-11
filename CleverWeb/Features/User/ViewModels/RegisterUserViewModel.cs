using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleverWeb.Features.Users.ViewModels
{
    public class RegisterUserViewModel
    {
        [Required]
        [Display(Name = "Usuário")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Tenant / Igreja")]
        public int TenantId { get; set; }

        [Required]
        [Display(Name = "Membro")]
        public int MembroId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas não conferem")]
        [Display(Name = "Confirmar Senha")]
        public string ConfirmarSenha { get; set; } = string.Empty;

        public List<SelectListItem> TenantOptions { get; set; } = new();
        public List<SelectListItem> MembroOptions { get; set; } = new();
    }
}
