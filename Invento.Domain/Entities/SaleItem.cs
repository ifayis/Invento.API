using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class SaleItem : BaseEntity
    {
        public Guid SaleId { get; set; }

        public Sale Sale { get; set; } = default!;

        public Guid ProductId { get; set; }

        public Product Product { get; set; } = default!;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TaxRate { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
