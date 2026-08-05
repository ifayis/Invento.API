using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Categories.DTOs;

namespace Invento.Application.Features.Categories.Commands
{
    public class CreateCategoryCommand 
        : ICommand<ApiResponse<CategoryDto>>
    {
        public string Name { get; set; } = null!;
    }
}
