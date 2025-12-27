using CleverWeb.Features.Membro.ViewModels;
using System.ComponentModel.DataAnnotations;
using static CleverWeb.Data.Shared.Enums;

namespace CleverWeb.Features.Contribuicao.ViewModels
{
    public class ContribuicaoViewModel
    {
        public int Id { get; set; }
        public int MembroId { get; set; }

        public Models.Membro? Membro { get; set; }

        [Required]
        [Range(0.01, 999999.99, ErrorMessage = "Informe um valor válido em reais.")]
        [DataType(DataType.Currency)]
        public decimal Valor { get; set; } = decimal.Parse("0,00");

        [Display(Name = "Contribuição")]
        public TipoContribuicao TipoContribuicao { get; set; }

        [Display(Name = "Forma de pagamento")]
        public FormaPagto FormaPagto { get; set; }

        [Display(Name = "Data de pagamento")]
        public DateTime DataPagamento { get; set; } = DateTime.Today;

        public DateTime DataLancamanto { get; set; } = DateTime.Today;
    }
}