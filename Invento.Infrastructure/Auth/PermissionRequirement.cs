using Microsoft.AspNetCore.Authorization;

namespace Invento.Infrastructure.Auth
{
    public class PermissionRequirement
        : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(
            string permission)
        {
            Permission = permission;
        }
    }
}