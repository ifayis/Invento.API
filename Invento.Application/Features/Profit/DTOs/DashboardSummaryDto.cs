namespace Invento.Application.Features.Profit.DTOs
{
    public class DashboardSummaryDto
    {
        public decimal TotalRevenue { get; set; }

        public decimal TotalProfit { get; set; }

        public int TotalSales { get; set; }

        public int TotalProducts { get; set; }

        public int LowStockProducts { get; set; }
    }
}
