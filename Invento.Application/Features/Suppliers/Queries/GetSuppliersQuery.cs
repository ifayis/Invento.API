using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Suppliers.DTOs;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.Suppliers.Queries
{
    public class GetSuppliersQuery
        : IQuery<ApiResponse<PagedResponse<SupplierDto>>>,
        ICacheableQuery
    {
        public string? Search { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string GetCacheKey()
        {
            return CacheKeys.Suppliers(
                CacheKeyBuilder.Build(this));
        }

    }
}