using static CleverWeb.Data.Shared.Enums;

namespace CleverWeb.Features.Caixa.ViewModels
{
    public class CaixaViewModel
    {
        public int Id { get; set; }
        public DateTime DtInicial { get; set; }
        public DateTime DtFinal { get; set; }
        public DateTime DtFechamento { get; set; }
        public decimal SaldoAtual { get; set; }
        public decimal SaldoAnterior { get; set; }
        public TipoContribuicao? TipoContribuicao { get; set; }
        public int UsuarioId { get; set; }
        public bool Cancelado { get; set; }
        public DateTime? DtCancelamento { get; set; }
        public string? MotivoCancelamento { get; set; }
    }
}