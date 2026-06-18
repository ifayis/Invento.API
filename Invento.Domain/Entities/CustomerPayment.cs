using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class CustomerPayment : BaseEntity
    {
        public Guid TenantId { get; set; }

        public Guid SaleId { get; set; }

        public Sale Sale { get; set; } = null!;

        public Guid CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Remarks { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
    }
}