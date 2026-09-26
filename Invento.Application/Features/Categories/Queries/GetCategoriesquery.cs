using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Categories.DTOs;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.Categories.Queries
{
    public class GetCategoriesQuery :
        IQuery<ApiResponse<PagedResponse<CategoryDto>>>
    {
        public string? Search { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public bool IncludeDeleted { get; set; } = true;
    }
}