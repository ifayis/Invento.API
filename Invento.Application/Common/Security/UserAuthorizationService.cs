using Invento.Domain.Enums;

namespace Invento.Application.Common.Security
{
    public static class UserAuthorizationService
    {
        public static bool CanCreateUser(
            UserRole currentRole,
            UserRole newUserRole)
        {
            return currentRole switch
            {
                UserRole.Admin => true,

                UserRole.Manager =>
                    newUserRole == UserRole.Staff,

                _ => false
            };
        }

        public static bool CanViewUser(
            UserRole currentRole,
            Guid currentTenantId,
            Guid currentUserId,
            UserRole targetRole,
            Guid targetTenantId,
            Guid targetUserId)
        {
            if (currentTenantId != targetTenantId)
            {
                return false;
            }

            if (currentRole == UserRole.Admin)
            {
                return true;
            }

            if (currentUserId == targetUserId)
            {
                return true;
            }

            if (currentRole == UserRole.Manager &&
                targetRole == UserRole.Staff)
            {
                return true;
            }

            return false;
        }

        public static bool CanEditProfile(
            Guid currentTenantId,
            Guid currentUserId,
            Guid targetTenantId,
            Guid targetUserId,
            UserRole currentRole)
        {
            if (currentTenantId != targetTenantId)
            {
                return false;
            }

            if (currentRole == UserRole.Admin)
            {
                return true;
            }

            return currentUserId == targetUserId;
        }

        public static bool CanManageUser(
            UserRole currentRole,
            Guid currentTenantId,
            Guid targetTenantId,
            UserRole targetRole)
        {
            if (currentTenantId != targetTenantId)
            {
                return false;
            }

            if (currentRole == UserRole.Admin)
            {
                return true;
            }

            if (currentRole == UserRole.Manager &&
                targetRole == UserRole.Staff)
            {
                return true;
            }

            return false;
        }

        public static bool CanChangeRole(
            UserRole currentRole)
        {
            return currentRole == UserRole.Admin;
        }

        public static bool CanDeleteUser(
            UserRole currentRole)
        {
            return currentRole == UserRole.Admin;
        }

        public static bool CanActivateUser(
            UserRole currentRole)
        {
            return currentRole == UserRole.Admin;
        }
    }
}