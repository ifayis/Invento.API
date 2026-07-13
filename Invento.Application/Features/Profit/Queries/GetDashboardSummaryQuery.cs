using Invento.Application.Abstractions;
using Invento.Application.Features.Profit.DTOs;
using Invento.Application.Common;
using Invento.Application.Common.Caching;

namespace Invento.Application.Features.Profit.Queries
{
    public class GetDashboardSummaryQuery
        : IQuery<ApiResponse<DashboardSummaryDto>>,
        ICacheableQuery
    {
        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string CacheGroup =>
            CacheGroups.Profit;

        public string GetCacheKey()
        {
            return CacheKeys.Profit(
                CacheKeyBuilder.Build(this));
        }
    }
}
