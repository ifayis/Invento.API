namespace Invento.Application.Features.Customers.DTOs
{
    public class InactiveCustomerDto
    {
        public Guid CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime? LastPurchaseDate { get; set; }

        public int TotalOrders { get; set; }

        public decimal TotalRevenue { get; set; }
    }
}