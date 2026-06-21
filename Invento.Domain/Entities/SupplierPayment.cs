using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class SupplierPayment : BaseEntity
    {
        public Guid TenantId { get; set; }

        public Guid SupplierId { get; set; }

        public Supplier Supplier { get; set; } = null!;

        public Guid PurchaseId { get; set; }

        public Purchase Purchase { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Remarks { get; set; }
            = string.Empty;

        public bool IsDeleted { get; set; }
    }
}