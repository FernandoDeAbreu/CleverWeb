namespace CleverWeb.Models
{
    public class Fornecedor
    {
        public int Id { get; set; }

        public string Descricao { get; set; } = string.Empty;

        public ICollection<Despesa> Despesas { get; set; }  
    }
}
