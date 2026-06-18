namespace Invento.Application.Features.Dashboard.DTOs
{
    public class DashboardSummaryDto
    {
        public decimal TotalSales { get; set; }

        public decimal TotalPurchases { get; set; }

        public decimal CurrentBalance { get; set; }

        public int TotalProducts { get; set; }

        public int LowStockProducts { get; set; }

        public int TotalCustomers { get; set; }

        public int TotalSuppliers { get; set; }
    }

}
