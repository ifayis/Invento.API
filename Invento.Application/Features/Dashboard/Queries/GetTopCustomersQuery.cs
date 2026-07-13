using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Dashboard.DTOs;

namespace Invento.Application.Features.Dashboard.Queries
{
    public class GetTopCustomersQuery
    : IQuery<ApiResponse<List<TopCustomerDto>>>,
        ICacheableQuery
    {
        public int Count { get; set; } = 10;

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
