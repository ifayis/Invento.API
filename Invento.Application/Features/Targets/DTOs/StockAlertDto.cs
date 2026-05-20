namespace Invento.Application.Features.Targets.DTOs
{
    public class StockAlertDto
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int CurrentStock { get; set; }

        public int Threshold { get; set; }
    }
}
