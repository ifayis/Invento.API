using Invento.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class StockMovement : AuditableEntity
    {
        public Guid ProductId { get; set; }

        public Product Product { get; set; } = default!;

        public StockMovementType Type { get; set; }

        public int Quantity { get; set; }

        public string ReferenceType { get; set; } = string.Empty;

        public Guid? ReferenceId { get; set; }

        public string? Notes { get; set; }
    }
}
