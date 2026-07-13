using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Users.DTOs;

namespace Invento.Application.Features.Users.Queries
{
    public class GetUserByIdQuery
        : IQuery<ApiResponse<UserDto>>,
        ICacheableQuery
    {
        public Guid Id { get; set; }

        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string CacheGroup =>
            CacheGroups.Users;

        public string GetCacheKey()
        {
            return CacheKeys.User(Id);
        }

    }
}