using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Customers.DTOs;

namespace Invento.Application.Features.Customers.Queries
{
    public class GetCustomerByIdQuery
        : IQuery<ApiResponse<CustomerDto>>,
        ICacheableQuery
    {
        public Guid Id { get; set; }

        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string CacheGroup =>
            CacheGroups.Customers;

        public string GetCacheKey()
        {
            return CacheKeys.Customer(Id);
        }

    }
}