using System.Text.Json;
using Invento.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Invento.Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(
            IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(
            string key)
        {
            var cachedData =
                await _cache.GetStringAsync(key);

            if (string.IsNullOrWhiteSpace(cachedData))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(
                cachedData);
        }

        public async Task SetAsync<T>(
            string key,
            T value,
            TimeSpan? expiry = null)
        {
            var options =
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        expiry ?? TimeSpan.FromMinutes(5)
                };

            var jsonData =
                JsonSerializer.Serialize(value);

            await _cache.SetStringAsync(
                key,
                jsonData,
                options);
        }

        public async Task RemoveAsync(
            string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}