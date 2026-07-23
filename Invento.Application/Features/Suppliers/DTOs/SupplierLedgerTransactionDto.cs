namespace Invento.Application.Features.Suppliers.DTOs
{
    public class SupplierLedgerTransactionDto
    {
        public DateTime Date { get; set; }

        public string TransactionType { get; set; } = string.Empty;

        public string ReferenceNumber { get; set; } = string.Empty;

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public decimal RunningBalance { get; set; }
    }
}