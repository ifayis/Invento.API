namespace Invento.Application.Features.Purchases.DTOs
{
    public class PurchaseDto
    {
        public Guid Id { get; set; }

        public Guid SupplierId { get; set; }

        public string SupplierName { get; set; }
            = string.Empty;

        public string PurchaseNumber { get; set; }
            = string.Empty;

        public DateTime PurchaseDate { get; set; }

        public decimal TotalAmount { get; set; }

        public bool IsDeleted { get; set; }
    }
}