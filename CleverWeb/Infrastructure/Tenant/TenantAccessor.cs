using System.Security.Claims;

namespace CleverWeb.Infrastructure.Tenant
{
    public class TenantAccessor : ITenantAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? CurrentTenantId
        {
            get
            {
                var context = _httpContextAccessor.HttpContext;
                var claim = context?.User?.FindFirst("tenant_id");

                if (claim == null || !int.TryParse(claim.Value, out var tenantId))
                    return null;

                return tenantId;
            }
        }
    }
}
