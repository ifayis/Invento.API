using Invento.Application.Abstractions;
using Invento.Domain.Enums;
using Invento.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.StockMovements.Commands
{
    public class CreateStockMovementCommand
        : ICommand<ApiResponse<Guid>>
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public StockMovementType MovementType { get; set; }

        public string? Remarks { get; set; }

        public string? ReferenceNumber { get; set; }
    }
}
