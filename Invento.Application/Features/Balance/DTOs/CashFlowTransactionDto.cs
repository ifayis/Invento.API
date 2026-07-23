namespace Invento.Application.Features.Balance.DTOs
{
    public class CashFlowTransactionDto
    {
        public DateTime Date { get; set; }

        public string TransactionType { get; set; } = string.Empty;

        public string ReferenceNumber { get; set; } = string.Empty;

        public decimal Income { get; set; }

        public decimal Expense { get; set; }
    }
}