using Invento.Application.Abstractions;
using Invento.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invento.Application.Common;

namespace Invento.Application.Features.Categories.Commands
{
    public class CreateStockMovementCommand
        : ICommand<ApiResponse<Guid>>
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public StockMovementType MovementType { get; set; }

        public string? Remarks { get; set; }
    }
}
