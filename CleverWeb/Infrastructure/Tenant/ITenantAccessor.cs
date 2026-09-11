namespace CleverWeb.Infrastructure.Tenant
{
    public interface ITenantAccessor
    {
        int? CurrentTenantId { get; }
    }
}
