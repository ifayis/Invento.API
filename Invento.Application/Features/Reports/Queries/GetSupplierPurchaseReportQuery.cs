using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Reports.DTOs;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetSupplierPurchaseReportQuery
        : IQuery<ApiResponse<List<SupplierPurchaseReportDto>>>,
        ICacheableQuery
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string GetCacheKey()
        {
            return CacheKeys.Reports(
                CacheKeyBuilder.Build(this));
        }

    }
}