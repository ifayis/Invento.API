using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class PurchaseItem : BaseEntity
    {
        public Guid TenantId { get; set; }

        public Guid PurchaseId { get; set; }

        public Purchase Purchase { get; set; }
            = null!;

        public Guid ProductId { get; set; }

        public Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal UnitCost { get; set; }

        public decimal TaxRate { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal TotalPrice { get; set; }
    }
}