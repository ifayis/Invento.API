namespace Invento.Application.Features.Dashboard.DTOs
{
    public class SalesTrendDto
    {
        public string Month { get; set; }
            = string.Empty;

        public decimal SalesAmount { get; set; }

        public decimal ProfitAmount { get; set; }
    }
}