namespace CleverWeb.Models
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string PastorCongregacional { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public ICollection<Membro> Membros { get; set; } = new List<Membro>();
        public ICollection<Contribuicao> Contribuicoes { get; set; } = new List<Contribuicao>();
        public ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();
        public ICollection<Fornecedor> Fornecedores { get; set; } = new List<Fornecedor>();
        public ICollection<Caixa> Caixas { get; set; } = new List<Caixa>();
    }
}
