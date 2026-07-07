using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; set; }

        public string Token { get; set; }
            = string.Empty;

        public Guid FamilyId { get; set; }

        public Guid? ReplacedByTokenId { get; set; }

        public byte[] RowVersion { get; set; }
            = Array.Empty<byte>();

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime? RevokedAt { get; set; }

        public User User { get; set; }
            = default!;
    }
}