using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Users.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Users.Queries
{
    public class GetMyProfileQuery
        : IQuery<ApiResponse<UserDto>>,
        ICacheableQuery
    {
        public TimeSpan Expiration =>
            CacheDurations.Long;

        public string CacheGroup =>
                CacheGroups.Users;

        public string GetCacheKey()
        {
            return CacheKeyBuilder.Build(this);
        }
    }
}