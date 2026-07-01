using System.Text.Json;
using Invento.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Invento.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redis;

    public RedisCacheService(
        IDistributedCache cache,
        IConnectionMultiplexer redis)
    {
        _cache = cache;
        _redis = redis;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var cachedData = await _cache.GetStringAsync(key);

        if (string.IsNullOrWhiteSpace(
            cachedData))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(
            cachedData,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy =
                    JsonNamingPolicy.CamelCase
            });
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiry = null)
    {
        var options =
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry
            };

        var jsonData = JsonSerializer.Serialize(
            value,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy =
                    JsonNamingPolicy.CamelCase
            });

        await _cache.SetStringAsync(
            key,
            jsonData,
            options
        );

    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}