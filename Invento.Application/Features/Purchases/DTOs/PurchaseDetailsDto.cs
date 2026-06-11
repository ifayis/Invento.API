namespace Invento.Application.Features.Purchases.DTOs
{
    public class PurchaseDetailsDto
    {
        public Guid Id { get; set; }

        public Guid SupplierId { get; set; }

        public string SupplierName { get; set; }
            = string.Empty;

        public string PurchaseNumber { get; set; }
            = string.Empty;

        public DateTime PurchaseDate { get; set; }

        public decimal SubTotal { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public bool IsDeleted { get; set; }

        public List<PurchaseItemDto> Items { get; set; }
            = new();
    }
}