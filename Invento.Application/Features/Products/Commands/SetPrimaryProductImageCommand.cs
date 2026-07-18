using Invento.Application.Abstractions;
using Invento.Application.Common;

namespace Invento.Application.Features.Products.Commands
{
    public class SetPrimaryProductImageCommand
        : ICommand<ApiResponse<string>>
    {
        public Guid ImageId { get; set; }
    }
}