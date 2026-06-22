namespace Invento.Application.Features.Dashboard.DTOs
{
    public class ProfitTrendDto
    {
        public string Month { get; set; }
            = string.Empty;

        public decimal Revenue { get; set; }

        public decimal Profit { get; set; }

        public decimal ProfitMargin { get; set; }
    }
}