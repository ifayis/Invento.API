namespace Invento.Application.Features.Balance.DTOs
{
    public class CashFlowDto
    {
        public decimal TotalIncome { get; set; }

        public decimal TotalExpense { get; set; }

        public decimal NetCashFlow { get; set; }

        public List<CashFlowTransactionDto> Transactions
        {
            get;
            set;
        } = new();
    }
}