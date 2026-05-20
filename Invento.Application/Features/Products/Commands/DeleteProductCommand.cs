using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Products.DTOs;

namespace Invento.Application.Features.Products.Commands
{
    public class DeleteProductCommand
        : ICommand<ApiResponse<ProductDto>>
    {
        public Guid Id { get; set; }
    }
}
