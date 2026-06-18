namespace Invento.Application.Features.Dashboard.DTOs
{
    public class RecentSaleDto
    {
        public Guid Id { get; set; }

        public string InvoiceNumber { get; set; }
        = string.Empty;

        public DateTime SaleDate { get; set; }

        public decimal TotalAmount { get; set; }
    }

}
