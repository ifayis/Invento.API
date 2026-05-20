using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Categories.DTOs;

namespace Invento.Application.Features.Categories.Commands
{
    public class DeleteCategoryCommand 
        : ICommand<ApiResponse<CategoryDto>>
    {
        public Guid Id { get; set; }
    }
}
