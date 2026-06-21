namespace Invento.Application.Features.Payables.DTOs
{
    public class SupplierOutstandingDto
    {
        public Guid SupplierId { get; set; }

        public string SupplierName { get; set; }
            = string.Empty;

        public decimal TotalPurchases { get; set; }

        public decimal TotalPaid { get; set; }

        public decimal OutstandingAmount { get; set; }
    }
}