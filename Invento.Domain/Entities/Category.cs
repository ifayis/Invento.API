using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class Category : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}

