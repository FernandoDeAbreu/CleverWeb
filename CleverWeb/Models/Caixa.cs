namespace CleverWeb.Models
{
    public class Caixa
    {
        public int Id { get; set; }
        public DateTime DtInicial { get; set; }
        public DateTime DtFinal { get; set; }
        public DateTime DtFechamento { get; set; }
        public decimal Saldo { get; set; }
        public int UsuarioId { get; set; }
    }
}