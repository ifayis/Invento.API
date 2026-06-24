using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public Guid TenantId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string EntityName { get; set; } = string.Empty;

        public string ActionType { get; set; } = string.Empty;

        public string RecordId { get; set; } = string.Empty;

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}