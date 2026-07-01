using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Reports.DTOs;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetLowStockProductsQuery
    : IQuery<ApiResponse<List<LowStockProductDto>>>,
        ICacheableQuery
    {
        public TimeSpan Expiration =>
            CacheDurations.Reports;

        public string GetCacheKey()
        {
            return CacheKeys.Reports(
                CacheKeyBuilder.Build(this));
        }
    }
}
