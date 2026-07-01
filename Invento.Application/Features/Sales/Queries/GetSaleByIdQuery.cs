using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Sales.DTOs;

namespace Invento.Application.Features.Sales.Queries
{
    public class GetSaleByIdQuery
        : IQuery<ApiResponse<SaleDetailsDto>>,
        ICacheableQuery
    {
        public Guid Id { get; set; }

        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string GetCacheKey()
        {
            return CacheKeys.Sale(Id);
        }

    }
}
