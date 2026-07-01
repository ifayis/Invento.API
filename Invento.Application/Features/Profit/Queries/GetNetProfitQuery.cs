using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Profit.DTOs;

namespace Invento.Application.Features.Profit.Queries
{
    public class GetNetProfitQuery
        : IQuery<ApiResponse<ProfitSummaryDto>>,
        ICacheableQuery
    {
        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string GetCacheKey()
        {
            return CacheKeys.Profit(
                CacheKeyBuilder.Build(this));
        }
    }
}
