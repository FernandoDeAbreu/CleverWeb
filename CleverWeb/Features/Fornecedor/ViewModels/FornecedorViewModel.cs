using System.ComponentModel.DataAnnotations;

namespace CleverWeb.Features.Fornecedor.ViewModels
{
    public class FornecedorViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe a descrição do fornecedor.")]
        [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres.")]
        [Display(Name = "Fornecedor")]
        public string Descricao { get; set; } = string.Empty;
    }
}
