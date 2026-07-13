using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Receivables.DTOs;

namespace Invento.Application.Features.Receivables.Queries
{
    public class GetCustomerPaymentHistoryQuery
        : IQuery<ApiResponse<List<CustomerPaymentHistoryDto>>>,
        ICacheableQuery
    {
        public Guid CustomerId { get; set; }

        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string CacheGroup =>
            CacheGroups.Receivables;

        public string GetCacheKey()
        {
            return CacheKeys.Receivables(
                CacheKeyBuilder.Build(this));
        }

    }
}