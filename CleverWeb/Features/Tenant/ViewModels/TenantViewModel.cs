using System.ComponentModel.DataAnnotations;

namespace CleverWeb.Features.Tenant.ViewModels
{
    public class TenantViewModel
    {
        [Required(ErrorMessage = "Informe o nome da igreja.")]
        [Display(Name = "Nome da igreja")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o identificador da igreja.")]
        [RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Use apenas letras minúsculas, números e hífen.")]
        [Display(Name = "Slug")]
        public string Slug { get; set; } = string.Empty;

        [Display(Name = "Endereço da igreja")]
        public string Endereco { get; set; } = string.Empty;

        [Display(Name = "Pastor congregacional")]
        public string PastorCongregacional { get; set; } = string.Empty;

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;
    }
}
