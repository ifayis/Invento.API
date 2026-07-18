using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class ProductImage : AuditableEntity
    {
        public Guid ProductId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string OriginalFileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }

        public Product Product { get; set; } = default!;
    }
}