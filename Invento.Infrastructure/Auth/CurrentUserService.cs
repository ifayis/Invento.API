using Invento.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Invento.Infrastructure.Auth
{
    public class CurrentUserService
        : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId =>
            _httpContextAccessor.HttpContext?
                .User?
                .FindFirst("Name")?
                .Value
            ?? "System";

        public Guid TenantId
        {
            get
            {
                var tenantId =
                    _httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst("TenantId")?
                        .Value;

                return Guid.TryParse(
                    tenantId,
                    out var id)
                    ? id
                    : Guid.Empty;
            }
        }

        public string Email =>
            _httpContextAccessor.HttpContext?
                .User?
                .FindFirst("Email")?
                .Value
            ?? string.Empty;

        public string Role =>
            _httpContextAccessor.HttpContext?
                .User?
                .FindFirst("Role")?
                .Value
            ?? string.Empty;
    }
}