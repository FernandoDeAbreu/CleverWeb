using CleverWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static CleverWeb.Data.Shared.Enums;

namespace CleverWeb.Features.Despesa.ViewModels
{
    public class DespesaViewModel
    {
        public int Id { get; set; }
        public int CaixaId { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 999999.99, ErrorMessage = "Informe um valor válido em reais.")]
        [DataType(DataType.Currency)]
        public decimal Valor { get; set; }

        [Display(Name = "Data do pagamento")]

        public DateTime DataPagamento { get; set; } = DateTime.Today;

        [Display(Name = "Saída do caixa")]

        public TipoContribuicao CaixaSaida { get; set; }

        [Display(Name = "Origem da despesa")]

        public int FornecedorId { get; set; }

        public Models.Fornecedor? Fornecedor { get; set; }

        public IEnumerable<SelectListItem>? FornecedorList { get; set; }

        public DateTime DataExclusao { get; set; }

        [Display(Name = "Motivo do estorno")]
        public string? MotivoExclusao { get; set; }
    }
}
