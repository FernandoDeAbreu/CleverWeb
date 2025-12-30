using CleverWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace CleverWeb.Data
{
    public class CleverDbContext(DbContextOptions<CleverDbContext> options) : DbContext(options)
    {
        public DbSet<Usuario> Usuario { get; set; }

        public DbSet<Membro> Membro { get; set; } = null!;
        public DbSet<Contribuicao> Contribuicao { get; set; } = null!;
        public DbSet<Despesa> Despesa { get; set; } = null!;
        public DbSet<Fornecedor> Fornecedor { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Membro>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Nome).HasMaxLength(200).IsRequired();
                b.Property(x => x.Email).HasMaxLength(200);
                b.Property(x => x.Telefone).HasMaxLength(50);
            });
            modelBuilder.Entity<Contribuicao>()
            .HasOne(c => c.Membro)
            .WithMany(m => m.Contribuicoes)
            .HasForeignKey(c => c.MembroId);
            modelBuilder.Entity<Despesa>()
            .HasOne(c => c.Fornecedor)
            .WithMany(m => m.Despesas)
            .HasForeignKey(c => c.FornecedorId);

        }
    }
}