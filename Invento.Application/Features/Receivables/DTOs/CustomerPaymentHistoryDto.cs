namespace Invento.Application.Features.Receivables.DTOs
{
    public class CustomerPaymentHistoryDto
    {
        public Guid Id { get; set; }

        public Guid SaleId { get; set; }

        public string InvoiceNumber { get; set; }
            = string.Empty;

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Remarks { get; set; }
            = string.Empty;
    }
}