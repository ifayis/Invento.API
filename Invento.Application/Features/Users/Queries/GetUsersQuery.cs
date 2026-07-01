using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Users.DTOs;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.Users.Queries
{
    public class GetUsersQuery
        : PaginationRequest,
          IQuery<ApiResponse<PagedResponse<UserDto>>>,
          ICacheableQuery
    {
        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string GetCacheKey()
        {
            return CacheKeys.Users(
                CacheKeyBuilder.Build(this));
        }
    }
}