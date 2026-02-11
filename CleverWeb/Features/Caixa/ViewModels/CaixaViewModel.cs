namespace CleverWeb.Features.Caixa.ViewModels
{
    public class CaixaViewModel
    {
        public int Id { get; set; }
        public DateTime DtInicial { get; set; }
        public DateTime DtFinal { get; set; }
        public DateTime DtFechamento { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoAtual { get; set; }
        public int UsuarioId { get; set; }
    }
}