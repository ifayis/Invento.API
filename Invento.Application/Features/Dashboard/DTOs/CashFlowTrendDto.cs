namespace Invento.Application.Features.Dashboard.DTOs
{
    public class CashFlowTrendDto
    {
        public string Month { get; set; }
            = string.Empty;

        public decimal CashIn { get; set; }

        public decimal CashOut { get; set; }

        public decimal NetCashFlow { get; set; }
    }
}