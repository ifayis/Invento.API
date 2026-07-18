namespace Invento.Application.Features.Products.DTOs
{
    public class ProductImageDto
    {
        public Guid Id { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string OriginalFileName { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }
    }
}