using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class StockAlert : BaseEntity
    {
        public Guid TenantId { get; set; }

        public Guid ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public string AlertType { get; set; } = string.Empty;

        public int CurrentStock { get; set; }

        public int ThresholdValue { get; set; }

        public bool IsResolved { get; set; }

        public bool IsDeleted { get; set; }
    }
}
