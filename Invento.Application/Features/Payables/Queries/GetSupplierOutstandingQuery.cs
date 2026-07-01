using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Payables.DTOs;

namespace Invento.Application.Features.Payables.Queries
{
    public class GetSupplierOutstandingQuery
        : IQuery<ApiResponse<List<SupplierOutstandingDto>>>,
        ICacheableQuery
    {
        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string GetCacheKey()
        {
            return CacheKeys.Payables(
                CacheKeyBuilder.Build(this));
        }
    }
}