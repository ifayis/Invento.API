namespace Invento.Application.Features.Dashboard.DTOs
{
    public class TopCustomerDto
    {
        public Guid CustomerId { get; set; }

        public string CustomerName { get; set; }
        = string.Empty;

        public int TotalOrders { get; set; }

        public decimal TotalSpent { get; set; }
    }

}
