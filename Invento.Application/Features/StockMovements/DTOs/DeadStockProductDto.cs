namespace Invento.Application.Features.StockMovements.DTOs
{
    public class DeadStockProductDto
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public int CurrentStock { get; set; }

        public decimal CostPrice { get; set; }

        public decimal InventoryValue { get; set; }

        public DateTime? LastSoldDate { get; set; }

        public int DaysWithoutSale { get; set; }
    }
}