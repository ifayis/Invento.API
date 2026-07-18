using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Products.DTOs;

namespace Invento.Application.Features.Products.Queries
{
    public class GetProductImagesQuery
        : IQuery<ApiResponse<List<ProductImageDto>>>
    {
        public Guid ProductId { get; set; }
    }
}