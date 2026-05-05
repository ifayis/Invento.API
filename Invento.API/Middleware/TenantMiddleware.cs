using Invento.Infrastructure.Services;

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
            var user = context.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                tenantProvider.SetTenantId(Guid.Parse(user.FindFirst("Name")!.Value));
                tenantProvider.SetUserId(Guid.Parse(user.FindFirst("UserId")!.Value));
            }

            await _next(context);
        }
    }
}
