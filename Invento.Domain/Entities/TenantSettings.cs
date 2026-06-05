using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class TenantSettings : BaseEntity
    {
        public Guid TenantId { get; set; }

        public decimal MonthlySalesTarget { get; set; }

        public decimal MonthlyProfitTarget { get; set; }

        public bool IsDeleted { get; set; }

        public Tenant Tenant { get; set; } = null!;
    }
}
