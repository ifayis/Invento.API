using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Customers.DTOs;

namespace Invento.Application.Features.Customers.Queries
{
    public class GetInactiveCustomersQuery
        : IQuery<ApiResponse<List<InactiveCustomerDto>>>,
        ICacheableQuery
    {
        public int Days { get; set; } = 30;

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