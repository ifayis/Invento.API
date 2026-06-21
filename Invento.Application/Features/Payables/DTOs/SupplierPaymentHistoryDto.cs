namespace Invento.Application.Features.Payables.DTOs
{
    public class SupplierPaymentHistoryDto
    {
        public Guid Id { get; set; }

        public Guid PurchaseId { get; set; }

        public string PurchaseNumber { get; set; }
            = string.Empty;

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Remarks { get; set; }
            = string.Empty;
    }
}