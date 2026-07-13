using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Categories.DTOs;

namespace Invento.Application.Features.Categories.Queries
{
    public class GetCategoryByIdQuery : 
        IQuery<ApiResponse<CategoryDto>>,
        ICacheableQuery
    {
        public Guid Id { get; set; }

        public TimeSpan Expiration =>
            CacheDurations.Short;

        public string CacheGroup =>
            CacheGroups.Categories;

        public string GetCacheKey()
        {
            return CacheKeys.Category(Id);
        }

    }
}
