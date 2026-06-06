using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.StockMovements.DTOs;

namespace Invento.Application.Features.StockMovements.Queries
{
    public class GetFastMovingProductsQuery
        : IQuery<ApiResponse<List<FastMovingProductDto>>>
    {
        public int Top { get; set; } = 10;
    }
}