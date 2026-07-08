using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class DocumentNumberSequence : BaseEntity
    {
        public Guid TenantId { get; set; }

        public string DocumentType { get; set; }
            = string.Empty;

        public string PeriodKey { get; set; }
            = string.Empty;

        public long NextNumber { get; set; }
    }
}