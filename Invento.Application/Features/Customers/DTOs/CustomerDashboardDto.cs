namespace Invento.Application.Features.Customers.DTOs
{
    public class CustomerDashboardDto
    {
        public int TotalCustomers { get; set; }

        public int ActiveCustomers { get; set; }

        public int InactiveCustomers { get; set; }

        public int NewCustomersThisMonth { get; set; }

        public decimal TopCustomerRevenue { get; set; }

        public decimal TopCustomerProfit { get; set; }
    }
}