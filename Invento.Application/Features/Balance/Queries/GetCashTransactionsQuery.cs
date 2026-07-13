using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Balance.DTOs;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.Balance.Queries
{
    public class GetCashTransactionsQuery
        : IQuery<ApiResponse<PagedResponse<CashTransactionDto>>>,
        ICacheableQuery
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string CacheGroup =>
            CacheGroups.Balance;

        public string GetCacheKey()
        {
            return CacheKeys.Balance(
                CacheKeyBuilder.Build(this));
        }

    }
}