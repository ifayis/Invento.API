using Hangfire.Dashboard;

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

            return user.IsInRole("Admin");
        }
    }
}