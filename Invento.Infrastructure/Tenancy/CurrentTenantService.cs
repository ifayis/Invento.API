using Invento.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Invento.Infrastructure.Tenancy;

public class CurrentTenantService : ICurrentTenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid TenantId
    {
        get
        {
            var tenantId =
                _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst("TenantId")
                ?.Value;

            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new UnauthorizedAccessException("Tenant not found");
            }

            return Guid.Parse(tenantId);
        }
    }
}