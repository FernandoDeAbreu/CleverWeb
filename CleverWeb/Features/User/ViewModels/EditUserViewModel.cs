using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleverWeb.Features.Users.ViewModels
{
    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Usuário")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Tenant / Igreja")]
        public int TenantId { get; set; }

        [Required]
        [Display(Name = "Membro")]
        public int MembroId { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;

        [Display(Name = "Admin Global")]
        public bool IsGlobalAdmin { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nova senha")]
        public string? NovaSenha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nova senha")]
        [Compare("NovaSenha", ErrorMessage = "As senhas não conferem")]
        public string? ConfirmarNovaSenha { get; set; }

        public List<SelectListItem> TenantOptions { get; set; } = new();
        public List<SelectListItem> MembroOptions { get; set; } = new();
    }
}
