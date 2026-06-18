namespace Invento.Application.Features.Reports.DTOs
{
    public class CustomerSalesReportDto
    {
        public Guid CustomerId { get; set; }

        public string CustomerName { get; set; }
            = string.Empty;

        public int TotalOrders { get; set; }

        public decimal TotalSales { get; set; }

        public decimal TotalProfit { get; set; }
    }
}