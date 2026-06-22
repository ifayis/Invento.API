namespace Invento.Application.Features.Dashboard.DTOs
{
    public class DashboardOverviewDto
    {
        public decimal TodaySales { get; set; }

        public decimal MonthlySales { get; set; }

        public decimal MonthlyProfit { get; set; }

        public decimal MonthlyPurchases { get; set; }

        public decimal OutstandingReceivables { get; set; }

        public decimal OutstandingPayables { get; set; }

        public decimal CashBalance { get; set; }

        public int LowStockProducts { get; set; }

        public int CriticalStockProducts { get; set; }
    }
}