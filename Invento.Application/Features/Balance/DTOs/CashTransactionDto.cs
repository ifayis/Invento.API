using Invento.Domain.Enums;

namespace Invento.Application.Features.Balance.DTOs
{
    public class CashTransactionDto
    {
        public Guid Id { get; set; }

        public CashTransactionType TransactionType { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }
            = string.Empty;

        public DateTime TransactionDate { get; set; }
    }
}