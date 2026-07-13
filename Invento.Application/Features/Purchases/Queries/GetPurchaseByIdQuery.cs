using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Purchases.DTOs;

namespace Invento.Application.Features.Purchases.Queries
{
    public class GetPurchaseByIdQuery
        : IQuery<ApiResponse<PurchaseDetailsDto>>,
        ICacheableQuery
    {
        public Guid Id { get; set; }

        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string CacheGroup =>
            CacheGroups.Purchases;

        public string GetCacheKey()
        {
            return CacheKeys.Purchase(Id);
        }

    }
}