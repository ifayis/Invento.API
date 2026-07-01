using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Customers.DTOs;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.Customers.Queries
{
    public class GetCustomersQuery
        : IQuery<ApiResponse<PagedResponse<CustomerDto>>>,
        ICacheableQuery
    {
        public string? Search { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string GetCacheKey()
        {
            return CacheKeys.Customers(
                CacheKeyBuilder.Build(this));
        }

    }
}
