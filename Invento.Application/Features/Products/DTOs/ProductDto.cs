namespace Invento.Application.Features.Products.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string SKU { get; set; }  = string.Empty;

        public decimal CostPrice { get; set; }
        
        public decimal SellingPrice { get; set; }

        public int CurrentStock { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
