namespace Invento.Application.Features.Targets.DTOs
{
    public class TenantTargetDto
    {
        public int LowStockThreshold { get; set; }

        public int CriticalStockThreshold { get; set; }

        public decimal MonthlySalesTarget { get; set; }

        public decimal MonthlyProfitTarget { get; set; }
    }
}
