namespace Invento.Application.Features.Receivables.DTOs
{
    public class CustomerPaymentDto
    {
        public Guid Id { get; set; }

        public Guid SaleId { get; set; }

        public Guid CustomerId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Remarks { get; set; }
            = string.Empty;
    }
}