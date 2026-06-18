namespace Invento.Application.Features.Reports.DTOs
{
    public class SupplierPurchaseReportDto
    {
        public Guid SupplierId { get; set; }

        public string SupplierName { get; set; }
            = string.Empty;

        public int TotalOrders { get; set; }

        public decimal TotalPurchases { get; set; }
    }
}