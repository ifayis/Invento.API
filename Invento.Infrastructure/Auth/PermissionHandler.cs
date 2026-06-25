using Microsoft.AspNetCore.Authorization;

namespace Invento.Infrastructure.Auth
{
    public class PermissionHandler
        : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var hasPermission =
                context.User.Claims.Any(
                    x =>
                        x.Type == "Permission"
                        && x.Value == requirement.Permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}