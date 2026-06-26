using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Users.DTOs;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.Users.Queries
{
    public class GetUsersQuery
        : PaginationRequest,
          IQuery<ApiResponse<PagedResponse<UserDto>>>
    {
    }
}