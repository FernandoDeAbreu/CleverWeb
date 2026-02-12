using static CleverWeb.Data.Shared.Enums;

namespace CleverWeb.Models
{
    public class Caixa
    {
        public int Id { get; set; }
        public DateTime DtInicial { get; set; }
        public DateTime DtFinal { get; set; }
        public DateTime DtFechamento { get; set; }
        public decimal SaldoAtual { get; set; }
        public decimal SaldoAnterior { get; set; }
        public TipoContribuicao? TipoContribuicao { get; set; }
        public int UsuarioId { get; set; }
    }
}