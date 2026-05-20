using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Categories.DTOs;

namespace Invento.Application.Features.Categories.Commands
{
    public class UpdateCategoryCommand 
        : ICommand<ApiResponse<CategoryDto>>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
    }
}
