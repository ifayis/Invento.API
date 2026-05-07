using Invento.Infrastructure.Services;
using Invento.Application.Common.Interface;

namespace Invento.API.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        
        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task invoke(HttpContext context, TenantProvider tenantProvider)
        {
            await _next(context);
        }
    }
}
