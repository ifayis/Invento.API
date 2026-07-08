using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class Product : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal CostPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal TaxRate { get; set; }

        public int CurrentStock { get; set; }

        public Guid CategoryId { get; set; }

        public int LowStockThreshold { get; set; } = 10;

        public int CriticalStockThreshold { get; set; } = 5;

        public byte[] RowVersion { get; set; } =
            Array.Empty<byte>();

        public Category Category { get; set; } = default!;

        public ICollection<ProductImage> Images { get; set; } =
            new List<ProductImage>();

        public ICollection<StockMovement> StockMovements { get; set; } =
            new List<StockMovement>();

        public ICollection<SaleItem> SaleItems { get; set; } =
            new List<SaleItem>();

        public ICollection<PurchaseItem> PurchaseItems { get; set; } =
            new List<PurchaseItem>();
    }
}