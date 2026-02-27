using static CleverWeb.Data.Shared.Enums;

namespace CleverWeb.Models
{
    public class Contribuicao
    {
        public int Id { get; set; }
        public int MembroId { get; set; }
        public Membro? Membro { get; set; }
        public decimal Valor { get; set; }
        public FormaPagto FormaPagto { get; set; }
        public TipoContribuicao TipoContribuicao { get; set; }
        public DateTime DataPagamento { get; set; }
        public DateTime DataLancamanto { get; set; }
        public DateTime DataExclusao { get; set; }
        public string? MotivoExclusao { get; set; }
        public int CaixaID { get; set; }
    }
}
