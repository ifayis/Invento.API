using Invento.Domain.Enums;
using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class User : AuditableEntity
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.Admin;

        public bool IsActive { get; set; } = true;

        public Tenant Tenant { get; set; } = default!;

        public virtual ICollection<RefreshToken> RefreshTokens
        { get; set; } = new List<RefreshToken>();

        public virtual ICollection<PasswordResetToken> PasswordResetTokens
        { get; set; } = new List<PasswordResetToken>();
    }
}
