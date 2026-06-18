namespace Invento.Application.Features.Reports.DTOs
{
    public class InventorySummaryReportDto
    {
        public int TotalProducts { get; set; }

        public int TotalStockQuantity { get; set; }

        public decimal InventoryValue { get; set; }

        public int LowStockProducts { get; set; }
    }

}
