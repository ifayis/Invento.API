using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.StockMovements.DTOs;

namespace Invento.Application.Features.StockMovements.Queries
{
    public class GetDeadStockProductsQuery
        : IQuery<ApiResponse<List<DeadStockProductDto>>>
    {
        public int Days { get; set; } = 90;
    }
}