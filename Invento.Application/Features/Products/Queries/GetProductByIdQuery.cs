using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Products.DTOs;

namespace Invento.Application.Features.Products.Queries
{
    public class GetProductByIdQuery
        : IQuery<ApiResponse<ProductDto>>,
        ICacheableQuery
    {
        public Guid Id { get; set; }

        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string CacheGroup =>
            CacheGroups.Products;

        public string GetCacheKey()
        {
            return CacheKeys.Product(Id);
        }

    }
}
