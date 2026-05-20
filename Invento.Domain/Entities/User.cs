using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class User : AuditableEntity
    {
        public Guid TenantId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Admin";

        public bool IsActive { get; set; } = true;

        public Tenant Tenant { get; set; } = default!;

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
