using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Products.DTOs;

namespace Invento.Application.Features.Products.Commands
{
    public class RestoreProductCommand
          : ICommand<ApiResponse<ProductStatusDto>>
    {
        public Guid Id { get; set; }
    }
}
