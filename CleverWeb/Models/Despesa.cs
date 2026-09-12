using static CleverWeb.Data.Shared.Enums;

namespace CleverWeb.Models
{
    public class Despesa
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        public string Descricao { get; set; } = string.Empty;

        public decimal Valor { get; set; } = decimal.Parse("0,00");

        public DateTime DataPagamento { get; set; } 

        public TipoContribuicao CaixaSaida { get; set; }

        public int FornecedorId { get; set; }

        public Fornecedor? Fornecedor { get; set; }

        public DateTime DataExclusao { get; set; }

        public string? MotivoExclusao { get; set; }

        public int CaixaId { get; set; }
        public bool GeradaNoFechamento { get; set; }
    }
}