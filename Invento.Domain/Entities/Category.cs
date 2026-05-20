using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class Category : AuditableEntity
    {
        public Guid TenantId { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

