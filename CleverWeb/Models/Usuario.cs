namespace CleverWeb.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
        public string UserName { get; set; } = "Abreu";
        public string PasswordHash { get; set; } = "fdas*2018";
        public bool Ativo { get; set; } = true;
    }
}
