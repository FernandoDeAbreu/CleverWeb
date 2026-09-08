using CleverWeb.Infrastructure.Tenant;
using CleverWeb.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CleverWeb.Data
{
    public class CleverDbContext : DbContext
    {
        private readonly ITenantAccessor _tenantAccessor;

        public CleverDbContext(DbContextOptions<CleverDbContext> options, ITenantAccessor tenantAccessor) : base(options)
        {
            _tenantAccessor = tenantAccessor;
        }

        public DbSet<Tenant> Tenant { get; set; } = null!;
        public DbSet<Usuario> Usuario { get; set; } = null!;
        public DbSet<Membro> Membro { get; set; } = null!;
        public DbSet<Contribuicao> Contribuicao { get; set; } = null!;
        public DbSet<Despesa> Despesa { get; set; } = null!;
        public DbSet<Fornecedor> Fornecedor { get; set; } = null!;
        public DbSet<Caixa> Caixa { get; set; } = null!;

        public IQueryable<TEntity> QueryByTenant<TEntity>(IQueryable<TEntity> query) where TEntity : class
        {
            var tenantId = _tenantAccessor.CurrentTenantId;
            if (!tenantId.HasValue)
                return query;

            return query.Where(entity => EF.Property<int>(entity, "TenantId") == tenantId.Value);
        }

        public override int SaveChanges()
        {
            ApplyTenantToEntities();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyTenantToEntities();
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tenant>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Nome).HasMaxLength(200).IsRequired();
                b.Property(x => x.Slug).HasMaxLength(100).IsRequired();
                b.HasIndex(x => x.Slug).IsUnique();
            });

            modelBuilder.Entity<Usuario>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasOne(x => x.Tenant)
                 .WithMany(x => x.Usuarios)
                 .HasForeignKey(x => x.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(x => new { x.TenantId, x.UserName }).IsUnique();
            });

            modelBuilder.Entity<Membro>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Nome).HasMaxLength(200).IsRequired();
                b.Property(x => x.Email).HasMaxLength(200);
                b.Property(x => x.Telefone).HasMaxLength(50);
                b.HasOne(x => x.Tenant)
                 .WithMany(x => x.Membros)
                 .HasForeignKey(x => x.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Contribuicao>(b =>
            {
                b.HasOne(c => c.Membro)
                 .WithMany(m => m.Contribuicoes)
                 .HasForeignKey(c => c.MembroId)
                 .OnDelete(DeleteBehavior.Restrict);
                b.HasOne(c => c.Tenant)
                 .WithMany(t => t.Contribuicoes)
                 .HasForeignKey(c => c.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Despesa>(b =>
            {
                b.HasOne(c => c.Fornecedor)
                 .WithMany(m => m.Despesas)
                 .HasForeignKey(c => c.FornecedorId)
                 .OnDelete(DeleteBehavior.Restrict);
                b.HasOne(c => c.Tenant)
                 .WithMany(t => t.Despesas)
                 .HasForeignKey(c => c.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Fornecedor>(b =>
            {
                b.HasOne(x => x.Tenant)
                 .WithMany(x => x.Fornecedores)
                 .HasForeignKey(x => x.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Caixa>(b =>
            {
                b.HasOne(x => x.Tenant)
                 .WithMany(x => x.Caixas)
                 .HasForeignKey(x => x.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            var tenantId = _tenantAccessor.CurrentTenantId;
            if (tenantId.HasValue)
            {
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    var clrType = entityType.ClrType;
                    if (clrType == typeof(Tenant))
                        continue;

                    if (clrType.GetProperty("TenantId") != null)
                    {
                        modelBuilder.Entity(clrType).HasQueryFilter(
                            CreateTenantFilter(clrType, tenantId.Value));
                    }
                }
            }
        }

        private void ApplyTenantToEntities()
        {
            var tenantId = _tenantAccessor.CurrentTenantId;
            if (!tenantId.HasValue)
                return;

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity == null)
                    continue;

                var tenantProperty = entry.Entity.GetType().GetProperty("TenantId");
                if (tenantProperty == null || tenantProperty.GetValue(entry.Entity) is not int || (int)tenantProperty.GetValue(entry.Entity)! != 0)
                    continue;

                if (entry.State == EntityState.Added)
                    tenantProperty.SetValue(entry.Entity, tenantId.Value);
            }
        }

        private static LambdaExpression CreateTenantFilter(Type type, int tenantId)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(type, "entity");
            var property = System.Linq.Expressions.Expression.Property(parameter, "TenantId");
            var constant = System.Linq.Expressions.Expression.Constant(tenantId);
            var comparison = System.Linq.Expressions.Expression.Equal(property, constant);
            return System.Linq.Expressions.Expression.Lambda(comparison, parameter);
        }
    }
}