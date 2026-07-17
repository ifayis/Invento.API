using Hangfire.Dashboard;
using Invento.Application.Common;

namespace Invento.API.Hangfire
{
    public class HangfireAuthorizationFilter
        : IDashboardAuthorizationFilter
    {
        public bool Authorize(
            DashboardContext context)
        {
            var httpContext =
                context.GetHttpContext();

            var user =
                httpContext.User;

            if (user?.Identity?.IsAuthenticated
                != true)
            {
                return false;
            }

            return
                user.Identity?.IsAuthenticated == true
                &&
                user.IsInRole("Admin")
                &&
                user.HasClaim(
                    "Permission",
                    Permissions.Dashboard);
        }
    }
}