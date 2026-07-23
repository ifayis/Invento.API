namespace Invento.Application.Features.Customers.DTOs
{
    public class CustomerLedgerDto
    {
        public Guid CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public decimal CurrentOutstanding { get; set; }

        public List<CustomerLedgerTransactionDto> Transactions
        {
            get;
            set;
        } = new();
    }
}