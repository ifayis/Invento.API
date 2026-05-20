using Invento.Application.Abstractions;
using Invento.Application.Features.Products.DTOs;
using Invento.Application.Common;

namespace Invento.Application.Features.Products.Queries
{
    public class GetProductByIdQuery
        : IQuery<ApiResponse<ProductDto>>
    {
        public Guid Id { get; set; }
    }
}
