using Invento.Application.Common.Interface;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Invento.Infrastructure.Services;

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetTenantId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        var tenantClaim = user?.FindFirst("Name")?.Value;

        if (tenantClaim == null)
            throw new UnauthorizedAccessException("Tenant not found in token");

        return Guid.Parse(tenantClaim);
    }

    public Guid GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        var userClaim = user?.FindFirst("UserId")?.Value;

        if (userClaim == null)
            throw new UnauthorizedAccessException("UserId not found in token");

        return Guid.Parse(userClaim);
    }
}