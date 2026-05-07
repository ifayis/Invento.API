using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }

        public string Name { get; set; } = string.Empty;

        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
