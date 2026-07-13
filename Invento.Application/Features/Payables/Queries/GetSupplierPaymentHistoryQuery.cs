using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Payables.DTOs;

namespace Invento.Application.Features.Payables.Queries
{
    public class GetSupplierPaymentHistoryQuery
        : IQuery<ApiResponse<List<SupplierPaymentHistoryDto>>>,
        ICacheableQuery
    {
        public Guid SupplierId { get; set; }

        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string CacheGroup =>
            CacheGroups.Payables;

        public string GetCacheKey()
        {
            return CacheKeys.Payables(
                CacheKeyBuilder.Build(SupplierId));
        }

    }
}