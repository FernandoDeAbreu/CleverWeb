using System.ComponentModel.DataAnnotations;

namespace CleverWeb.Features.Auth.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Usuário")]
        public string UserName { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = null!;

    }
}
