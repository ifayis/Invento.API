namespace Invento.Application.Features.Receivables.DTOs
{
    public class CustomerOutstandingDto
    {
        public Guid CustomerId { get; set; }

        public string CustomerName { get; set; }
            = string.Empty;

        public decimal TotalSales { get; set; }

        public decimal TotalPaid { get; set; }

        public decimal OutstandingAmount { get; set; }
    }
}