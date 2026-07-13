using Invento.Application.Common.Caching;
using Invento.Application.Interfaces;
using MediatR;

namespace Invento.Application.Behaviours
{
    public class CachingBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ICacheService _cacheService;
        private readonly ICurrentTenantService _currentTenant;
        private readonly ICacheVersionService _cacheVersionService;

        public CachingBehavior(
            ICacheService cacheService,
            ICurrentTenantService currentTenant,
            ICacheVersionService cacheVersionService)
        {
            _cacheService = cacheService;
            _currentTenant = currentTenant;
            _cacheVersionService =  cacheVersionService;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is not ICacheableQuery cacheableQuery)
            {
                return await next();
            }

            var tenantId =
                _currentTenant.TenantId;

            var version =
                await _cacheVersionService
                    .GetVersionAsync(
                        tenantId,
                        cacheableQuery.CacheGroup,
                        cancellationToken);

            var cacheKey =
                $"{tenantId:N}:"
                + $"{cacheableQuery.CacheGroup}:"
                + $"v{version}:"
                + cacheableQuery.GetCacheKey();

            var cachedResponse =
                await _cacheService.GetAsync<TResponse>(
                    cacheKey);

            if (cachedResponse is not null)
            {
                Console.WriteLine($"CACHE HIT : {cacheKey}");
                return cachedResponse;
            }

            Console.WriteLine($"CACHE MISS : {cacheKey}");

            var response =
                await next();

            await _cacheService.SetAsync(
                cacheKey,
                response,
                cacheableQuery.Expiration);

            return response;
        }
    }
}