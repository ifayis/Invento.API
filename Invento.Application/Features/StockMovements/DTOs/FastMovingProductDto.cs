namespace Invento.Application.Features.StockMovements.DTOs
{
    public class FastMovingProductDto
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public int TotalQuantitySold { get; set; }

        public decimal TotalRevenue { get; set; }

        public decimal TotalProfit { get; set; }

        public int CurrentStock { get; set; }
    }
}