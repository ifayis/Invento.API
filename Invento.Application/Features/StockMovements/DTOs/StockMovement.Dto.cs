using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.StockMovements.DTOs
{
    public class StockMovementDto
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; }
            = string.Empty;

        public int Quantity { get; set; }

        public string MovementType { get; set; }
            = string.Empty;

        public string? Remarks { get; set; }

        public string? ReferenceNumber { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
