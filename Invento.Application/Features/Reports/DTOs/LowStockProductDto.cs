namespace Invento.Application.Features.Reports.DTOs
{
    public class LowStockProductDto
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }
            = string.Empty;

        public string SKU { get; set; }
            = string.Empty;

        public int CurrentStock { get; set; }

        public int LowStockThreshold { get; set; }
    }
}
