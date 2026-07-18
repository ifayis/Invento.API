using Invento.Application.Abstractions;
using Invento.Application.Common;
using Microsoft.AspNetCore.Http;

namespace Invento.Application.Features.Products.Commands
{
    public class UploadProductImageCommand
        : ICommand<ApiResponse<Guid>>
    {
        public Guid ProductId { get; set; }

        public IFormFile Image { get; set; } = default!;
    }
}