namespace Invento.Application.Features.Targets.DTOs
{
    public class StockAlertDto
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int CurrentStock { get; set; }

        public int LowStockThreshold { get; set; }

        public int CriticalStockThreshold { get; set; }

        public bool IsDeleted { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
