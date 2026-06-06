namespace Invento.Application.Features.Targets.DTOs
{
    public class ReorderProductDto
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public int CurrentStock { get; set; }

        public int LowStockThreshold { get; set; }

        public int RecommendedOrderQuantity { get; set; }
    }
}