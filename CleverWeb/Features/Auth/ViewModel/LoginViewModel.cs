using System.ComponentModel.DataAnnotations;

namespace CleverWeb.Features.Auth.ViewModel
{
    public class LoginViewModel
    {
        [Display(Name = "Empresa")]
        public int? TenantId { get; set; }

        [Required]
        [Display(Name = "Usuário")]
        public string UserName { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = null!;
    }
}
