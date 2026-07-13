using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Company.DTOs;

namespace Invento.Application.Features.Company.Queries
{
    public class GetCompanyProfileQuery
        : IQuery<ApiResponse<CompanyProfileDto>>,
        ICacheableQuery
    {
        public TimeSpan Expiration =>
            CacheDurations.Long;

        public string CacheGroup =>
            CacheGroups.Company;

        public string GetCacheKey()
        {
            return CacheKeys.Company();
        }
    }
}
