namespace Invento.Application.Features.Dashboard.DTOs
{
    public class TopSupplierDto
    {
        public Guid SupplierId { get; set; }

        public string SupplierName { get; set; }
        = string.Empty;

        public int TotalPurchases { get; set; }

        public decimal TotalAmount { get; set; }
    }

}
