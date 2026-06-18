namespace Invento.Application.Features.Dashboard.DTOs
{
    public class RecentPurchaseDto
    {
        public Guid Id { get; set; }

        public string PurchaseNumber { get; set; }
        = string.Empty;

        public DateTime PurchaseDate { get; set; }

        public decimal TotalAmount { get; set; }
    }

}
