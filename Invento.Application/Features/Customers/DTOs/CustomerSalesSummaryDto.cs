namespace Invento.Application.Features.Customers.DTOs
{
    public class CustomerSalesSummaryDto
    {
        public Guid CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public int TotalOrders { get; set; }

        public decimal TotalRevenue { get; set; }

        public decimal TotalProfit { get; set; }

        public DateTime? FirstPurchaseDate { get; set; }

        public DateTime? LastPurchaseDate { get; set; }

        public List<CustomerSaleHistoryDto> RecentSales { get; set; }
            = new();
    }
}