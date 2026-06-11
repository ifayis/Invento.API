namespace Invento.Application.Features.Purchases.DTOs
{
    public class CreatePurchaseItemDto
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitCost { get; set; }

        public decimal TaxRate { get; set; }
    }
}