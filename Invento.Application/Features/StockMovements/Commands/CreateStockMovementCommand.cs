using Invento.Application.Abstractions;
using Invento.Domain.Enums;
using Invento.Application.Common;
using Invento.Application.Features.StockMovements.DTOs;

namespace Invento.Application.Features.StockMovements.Commands
{
    public class CreateStockMovementCommand
        : ICommand<ApiResponse<StockMovementDto>>
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public StockMovementType MovementType { get; set; }

        public string? Remarks { get; set; }

        public string? ReferenceNumber { get; set; }
    }
}
