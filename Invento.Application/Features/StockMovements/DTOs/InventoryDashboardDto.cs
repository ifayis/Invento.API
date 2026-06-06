namespace Invento.Application.Features.StockMovements.DTOs
{
    public class InventoryDashboardDto
    {
        public int TotalStockIn { get; set; }

        public int TotalStockOut { get; set; }

        public int TotalAdjustments { get; set; }

        public int TotalProducts { get; set; }

        public int LowStockProducts { get; set; }

        public int CriticalStockProducts { get; set; }
    }
}
