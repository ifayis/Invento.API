using Invento.Shared.Common;
using System.ComponentModel.DataAnnotations;

namespace Invento.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; set; }

        public string Token { get; set; }
            = string.Empty;

        public Guid FamilyId { get; set; }

        public Guid? ReplacedByTokenId { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime? RevokedAt { get; set; }

        public User User { get; set; }
            = default!;
    }
}