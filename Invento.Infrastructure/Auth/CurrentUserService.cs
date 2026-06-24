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

        public string? UserId =>
            _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirst("Name")
                ?.Value;
    }
}