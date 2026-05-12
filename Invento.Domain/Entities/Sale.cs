using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class Sale : AuditableEntity
    {
        public string SaleNumber { get; set; } = string.Empty;

        public Guid CustomerId { get; set; }

        public Customer Customer { get; set; } = default!;

        public DateTime SaleDate { get; set; }

        public decimal SubTotal { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public ICollection<SaleItem> Items { get; set; }
            = new List<SaleItem>();
    }
}
