namespace Invento.Application.Features.Reports.DTOs
{
    public class FinancialDashboardDto
    {
        public decimal TotalSalesRevenue { get; set; }

        public decimal TotalPurchaseCost { get; set; }

        public decimal TotalProfit { get; set; }

        public decimal CashInflow { get; set; }

        public decimal CashOutflow { get; set; }

        public decimal NetCashFlow { get; set; }

        public decimal TotalReceivables { get; set; }

        public decimal TotalPayables { get; set; }
    }
}