using Invento.Application.Abstractions;
using Invento.Application.Features.Categories.DTOs;
using Invento.Application.Common;

namespace Invento.Application.Features.Categories.Queries
{
    public class GetCategoryByIdQuery : IQuery<ApiResponse<CategoryDto>>
    {
        public Guid Id { get; set; }
    }
}
