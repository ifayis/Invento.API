namespace Invento.Application.Features.Reports.DTOs
{
    public class SalesSummaryReportDto
    {
        public decimal TotalSales { get; set; }

        public decimal TotalProfit { get; set; }

        public int TotalOrders { get; set; }

        public decimal AverageOrderValue { get; set; }
    }
}