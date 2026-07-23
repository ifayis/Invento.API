using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Products.DTOs;

namespace Invento.Application.Features.Products.Queries
{
    public class GetProductSalesHistoryQuery
        : IQuery<ApiResponse<List<ProductSalesHistoryDto>>>
    {
        public Guid ProductId { get; set; }
    }
}