namespace Invento.Application.Features.Balance.DTOs
{
    public class BalanceDashboardDto
    {
        public decimal TotalIncome { get; set; }

        public decimal TotalExpense { get; set; }

        public decimal CurrentBalance { get; set; }

        public decimal SalesIncome { get; set; }

        public decimal PurchaseExpense { get; set; }

        public decimal ManualIncome { get; set; }

        public decimal ManualExpense { get; set; }
    }
}