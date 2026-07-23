using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Products.DTOs;

namespace Invento.Application.Features.Products.Queries
{
    public class GetProductStockHistoryQuery
        : IQuery<ApiResponse<List<ProductStockHistoryDto>>>
    {
        public Guid ProductId { get; set; }
    }
}