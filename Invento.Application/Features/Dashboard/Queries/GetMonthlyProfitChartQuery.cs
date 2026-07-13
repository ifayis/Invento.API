using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Dashboard.DTOs;

namespace Invento.Application.Features.Dashboard.Queries
{
    public class GetMonthlyProfitChartQuery
    : IQuery<ApiResponse<List<MonthlyChartDto>>>,
        ICacheableQuery
    {
        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string CacheGroup =>
            CacheGroups.Dashboard;

        public string GetCacheKey()
        {
            return CacheKeys.Dashboard();
        }

    }
}
