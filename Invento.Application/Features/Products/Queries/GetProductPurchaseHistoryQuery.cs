using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Products.DTOs;

namespace Invento.Application.Features.Products.Queries
{
    public class GetProductPurchaseHistoryQuery
        : IQuery<ApiResponse<List<ProductPurchaseHistoryDto>>>
    {
        public Guid ProductId { get; set; }
    }
}