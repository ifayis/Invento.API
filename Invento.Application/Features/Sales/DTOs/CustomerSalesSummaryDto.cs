namespace Invento.Application.Features.Sales.DTOs
{
    public class CustomerSalesSummaryDto
    {
        public Guid CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public int TotalOrders { get; set; }

        public decimal TotalSalesAmount { get; set; }

        public decimal TotalProfit { get; set; }
    }
}