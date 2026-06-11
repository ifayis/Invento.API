namespace Invento.Application.Features.Purchases.DTOs
{
    public class PurchaseItemDto
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }
            = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitCost { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal TotalPrice { get; set; }
    }
}