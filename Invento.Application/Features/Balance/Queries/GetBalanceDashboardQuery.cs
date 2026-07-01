using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Balance.DTOs;

namespace Invento.Application.Features.Balance.Queries
{
    public class GetBalanceDashboardQuery
        : IQuery<ApiResponse<BalanceDashboardDto>>,
        ICacheableQuery
    {
        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string GetCacheKey()
        {
            return CacheKeys.Balance(
                CacheKeyBuilder.Build(this));
        }
    }
}