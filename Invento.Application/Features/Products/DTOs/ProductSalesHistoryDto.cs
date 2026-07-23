namespace Invento.Application.Features.Products.DTOs
{
    public class ProductSalesHistoryDto
    {
        public Guid SaleId { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;

        public DateTime SaleDate { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal ProfitAmount { get; set; }

        public string CustomerName { get; set; } = string.Empty;
    }
}