namespace Invento.Application.Features.Customers.DTOs
{
    public class CustomerSaleHistoryDto
    {
        public Guid SaleId { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;

        public DateTime SaleDate { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal ProfitAmount { get; set; }
    }
}