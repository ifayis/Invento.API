using Invento.Application.Abstractions;
using Invento.Application.Features.Categories.DTOs;
using Invento.Application.Common;

namespace Invento.Application.Features.Categories.Queries
{
    public class GetCategoriesQuery : IQuery<ApiResponse<PagedResponse<CategoryDto>>>
    {
        public string? Search { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
