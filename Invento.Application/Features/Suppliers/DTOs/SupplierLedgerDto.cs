namespace Invento.Application.Features.Suppliers.DTOs
{
    public class SupplierLedgerDto
    {
        public Guid SupplierId { get; set; }

        public string SupplierName { get; set; } = string.Empty;

        public decimal CurrentOutstanding { get; set; }

        public List<SupplierLedgerTransactionDto> Transactions
        {
            get;
            set;
        } = new();
    }
}