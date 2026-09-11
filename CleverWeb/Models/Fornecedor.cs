namespace CleverWeb.Models
{
    public class Fornecedor
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        public string Descricao { get; set; } = string.Empty;

        public ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();
    }
}
