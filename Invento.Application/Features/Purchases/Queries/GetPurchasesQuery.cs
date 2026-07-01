using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Purchases.DTOs;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.Purchases.Queries
{
    public class GetPurchasesQuery
        : IQuery<ApiResponse<PagedResponse<PurchaseDto>>>,
        ICacheableQuery
    {
        public string? Search { get; set; }

        public Guid? SupplierId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string GetCacheKey()
        {
            return CacheKeys.Purchases(
                CacheKeyBuilder.Build(this));
        }

    }
}