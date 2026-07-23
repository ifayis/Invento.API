namespace Invento.Application.Features.Products.DTOs
{
    public class ProductStockHistoryDto
    {
        public Guid StockMovementId { get; set; }

        public DateTime MovementDate { get; set; }

        public string MovementType { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public int CurrentStockAfterMovement { get; set; }

        public string? ReferenceNumber { get; set; }

        public string? Remarks { get; set; }
    }
}