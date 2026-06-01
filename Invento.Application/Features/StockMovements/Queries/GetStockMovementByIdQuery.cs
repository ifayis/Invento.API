using Invento.Application.Abstractions;
using Invento.Application.Common;

namespace Invento.Application.Features.StockMovements.Queries
{
    public class GetStockMovementByIdQuery
        : IQuery<ApiResponse<StockMovementDto>>
    {
        public Guid Id { get; set; }
    }
}
