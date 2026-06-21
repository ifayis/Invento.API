namespace Invento.Application.Features.Payables.DTOs
{
    public class SupplierPaymentDto
    {
        public Guid Id { get; set; }

        public Guid PurchaseId { get; set; }

        public Guid SupplierId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Remarks { get; set; }
            = string.Empty;
    }
}