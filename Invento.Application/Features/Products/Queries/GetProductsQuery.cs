using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Products.DTOs;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.Products.Queries
{
    public class GetProductsQuery
        : IQuery<ApiResponse<PagedResponse<ProductDto>>>,
        ICacheableQuery
    {
        public string? Search { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string GetCacheKey()
        {
            return CacheKeys.Products(
                CacheKeyBuilder.Build(this));
        }

    }
}
