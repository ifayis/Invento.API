using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Dashboard.DTOs;

namespace Invento.Application.Features.Dashboard.Queries
{
    public class GetDashboardOverviewQuery
        : IQuery<ApiResponse<DashboardOverviewDto>>,
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