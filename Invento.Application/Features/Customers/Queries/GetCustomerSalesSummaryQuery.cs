using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Customers.DTOs;

namespace Invento.Application.Features.Customers.Queries
{
    public class GetCustomerSalesSummaryQuery
        : IQuery<ApiResponse<CustomerSalesSummaryDto>>,
        ICacheableQuery
    {
        public Guid CustomerId { get; set; }

        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string CacheGroup =>
            CacheGroups.Categories;

        public string GetCacheKey()
        {
            return CacheKeys.Categories(
                CacheKeyBuilder.Build(CustomerId));
        }

    }
}