namespace CleverWeb.Models
{
    public class Caixa
    {
        public int Id { get; set; }
        public DateTime DtFechamento { get; set; }
        public decimal Saldo { get; set; }
        public int UsuarioId { get; set; }
    }
}