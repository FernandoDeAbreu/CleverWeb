namespace CleverWeb.Models
{
    public class Caixa
    {
        public int Id { get; set; }
        public DateTime DtFechamento { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoAtual { get; set; }
        public int UsuarioId { get; set; }
    }
}