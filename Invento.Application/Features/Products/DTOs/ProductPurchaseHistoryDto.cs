namespace Invento.Application.Features.Products.DTOs
{
    public class ProductPurchaseHistoryDto
    {
        public Guid PurchaseId { get; set; }

        public string PurchaseNumber { get; set; } = string.Empty;

        public DateTime PurchaseDate { get; set; }

        public int Quantity { get; set; }

        public decimal UnitCost { get; set; }

        public decimal TotalPrice { get; set; }

        public string SupplierName { get; set; } = string.Empty;
    }
}