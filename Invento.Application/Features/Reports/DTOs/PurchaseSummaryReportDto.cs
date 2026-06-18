namespace Invento.Application.Features.Reports.DTOs
{
    public class PurchaseSummaryReportDto
    {
        public decimal TotalPurchases { get; set; }

        public int TotalPurchaseOrders { get; set; }

        public decimal AveragePurchaseValue { get; set; }
    }
}