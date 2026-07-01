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

        public CachingBehavior(
            ICacheService cacheService,
            ICurrentTenantService currentTenant)
        {
            _cacheService = cacheService;
            _currentTenant = currentTenant;
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

            var cacheKey =
                $"{tenantId}:{cacheableQuery.GetCacheKey()}";

            var cachedResponse =
                await _cacheService.GetAsync<TResponse>(
                    cacheKey);

            if (cachedResponse is not null)
            {
                return cachedResponse;
            }

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