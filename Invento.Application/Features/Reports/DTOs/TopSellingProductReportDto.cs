namespace Invento.Application.Features.Reports.DTOs
{
    public class TopSellingProductReportDto
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }
            = string.Empty;

        public int TotalQuantitySold { get; set; }

        public decimal TotalRevenue { get; set; }

        public decimal TotalProfit { get; set; }
    }
}