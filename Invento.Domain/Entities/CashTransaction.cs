using Invento.Domain.Enums;
using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class CashTransaction : BaseEntity
    {
        public Guid TenantId { get; set; }

        public CashTransactionType TransactionType { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }
            = string.Empty;

        public DateTime TransactionDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}