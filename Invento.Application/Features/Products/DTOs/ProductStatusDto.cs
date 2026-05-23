namespace Invento.Application.Features.Products.DTOs
{
    public class ProductStatusDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
    }
}