using CleverWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static CleverWeb.Data.Shared.Enums;

namespace CleverWeb.Features.Despesa.ViewModels
{
    public class DespesaViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; } = string.Empty;

        public decimal Valor { get; set; } = decimal.Parse("0,00");

        [Display(Name = "Data do pagamento")]

        public DateTime DataPagamento { get; set; } = DateTime.Today;

        [Display(Name = "Saída do caixa")]

        public TipoContribuicao CaixaSaida { get; set; }

        [Display(Name = "Origem da despesa")]

        public int FornecedorId { get; set; }

        public Fornecedor? Fornecedor { get; set; }

        public IEnumerable<SelectListItem>? FornecedorList { get; set; }

        public DateTime DataExclusao { get; set; }

        public string? MotivoExclusao { get; set; }
    }
}
