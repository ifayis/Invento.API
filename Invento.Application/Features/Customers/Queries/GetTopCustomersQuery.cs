using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Customers.DTOs;

namespace Invento.Application.Features.Customers.Queries
{
    public class GetTopCustomersQuery
        : IQuery<ApiResponse<List<CustomerSalesSummaryDto>>>,
        ICacheableQuery
    {
        public int Top { get; set; } = 10;

        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string CacheGroup =>
            CacheGroups.Categories;

        public string GetCacheKey()
        {
            return CacheKeys.Categories(
                CacheKeyBuilder.Build(this));
        }

    }
}