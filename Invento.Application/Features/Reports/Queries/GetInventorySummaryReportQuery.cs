using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Reports.DTOs;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetInventorySummaryReportQuery
    : IQuery<ApiResponse<InventorySummaryReportDto>>,
        ICacheableQuery
    {
        public TimeSpan Expiration =>
            CacheDurations.Reports;

        public string CacheGroup =>
            CacheGroups.Reports;

        public string GetCacheKey()
        {
            return CacheKeys.Reports(
                CacheKeyBuilder.Build(this));
        }
    }
}