using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class TenantSettings : BaseEntity
    {
        public Guid TenantId { get; set; }

        public int LowStockThreshold { get; set; } = 10;

        public int CriticalStockThreshold { get; set; } = 3;

        public decimal MonthlySalesTarget { get; set; }

        public decimal MonthlyProfitTarget { get; set; }

        public bool IsDeleted { get; set; }

        public Tenant Tenant { get; set; } = null!;
    }
}
