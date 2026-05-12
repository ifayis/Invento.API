using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class ProductImage : BaseEntity
    {
        public Guid ProductId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public Product Product { get; set; } = default!;
    }
}
