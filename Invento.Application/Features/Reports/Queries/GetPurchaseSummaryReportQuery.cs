using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Reports.DTOs;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetPurchaseSummaryReportQuery
        : IQuery<ApiResponse<PurchaseSummaryReportDto>>
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public TimeSpan Expiration =>
            CacheDurations.Reports;

        public string CacheGroup =>
            CacheGroups.Reports;

        public string GetCacheKey()
        {
            return CacheKeyBuilder.Build(this);
        }
    }
}