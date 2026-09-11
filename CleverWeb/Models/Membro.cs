namespace CleverWeb.Models
{
    public class Membro
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string Telefone { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public ICollection<Contribuicao> Contribuicoes { get; set; } = new List<Contribuicao>();
    }
}
