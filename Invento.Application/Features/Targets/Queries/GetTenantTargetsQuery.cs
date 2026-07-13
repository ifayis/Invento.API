using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Targets.DTOs;

namespace Invento.Application.Features.Targets.Queries
{
    public class GetTenantTargetsQuery
        : IQuery<ApiResponse<TenantTargetDto>>,
        ICacheableQuery
    {
        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string CacheGroup =>
            CacheGroups.Targets;

        public string GetCacheKey()
        {
            return CacheKeys.Targets(
                CacheKeyBuilder.Build(this));
        }
    }
}
