using Invento.Application.Common.Caching;
using Invento.Application.Interfaces;

namespace Invento.Application.Common.Extensions;

public static class CacheInvalidationExtensions
{
    public static async Task InvalidateAsync(
        this ICacheVersionService cacheVersionService,
        Guid tenantId,
        params string[] groups)
    {
        foreach (var group in groups.Distinct())
        {
            await cacheVersionService
                .IncrementVersionAsync(
                    tenantId,
                    group);
        }
    }
}