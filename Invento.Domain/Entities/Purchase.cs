using Invento.Domain.Enums;
using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class Purchase : BaseEntity
    {
        public Guid TenantId { get; set; }

        public string PurchaseNumber { get; set; }
            = string.Empty;

        public Guid SupplierId { get; set; }

        public Supplier Supplier { get; set; }
            = null!;

        public DateTime PurchaseDate { get; set; }

        public decimal SubTotal { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal DueAmount { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<PurchaseItem> PurchaseItems
        { get; set; } = new List<PurchaseItem>();

        public ICollection<SupplierPayment> Payments
        { get; set; } = new List<SupplierPayment>();
    }
}