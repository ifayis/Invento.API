using System.Text.Json;
using Invento.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Invento.Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private static readonly JsonSerializerOptions
            SerializerOptions =
                new(JsonSerializerDefaults.Web);

        private readonly IDistributedCache _cache;

        private readonly ILogger<RedisCacheService>
            _logger;

        public RedisCacheService(
            IDistributedCache cache,
            ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(
            string key,
            CancellationToken cancellationToken = default)
        {
            ValidateKey(key);

            try
            {
                var cachedData =
                    await _cache.GetAsync(
                        key,
                        cancellationToken);

                if (cachedData is null ||
                    cachedData.Length == 0)
                {
                    return default;
                }

                return JsonSerializer.Deserialize<T>(
                    cachedData,
                    SerializerOptions);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Invalid cached JSON for key {CacheKey}. Removing cache entry.",
                    key);

                try
                {
                    await _cache.RemoveAsync(
                        key,
                        cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception removeException)
                {
                    _logger.LogWarning(
                        removeException,
                        "Failed to remove invalid cache entry {CacheKey}.",
                        key);
                }

                return default;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Cache read failed for key {CacheKey}. Falling back to source.",
                    key);

                return default;
            }
        }

        public async Task SetAsync<T>(
            string key,
            T value,
            TimeSpan? expiry = null,
            CancellationToken cancellationToken = default)
        {
            ValidateKey(key);

            var effectiveExpiry =
                expiry ?? TimeSpan.FromMinutes(5);

            if (effectiveExpiry <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(expiry),
                    "Cache expiry must be greater than zero.");
            }

            try
            {
                var data =
                    JsonSerializer.SerializeToUtf8Bytes(
                        value,
                        SerializerOptions);

                var options =
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow =
                            effectiveExpiry
                    };

                await _cache.SetAsync(
                    key,
                    data,
                    options,
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Cache write failed for key {CacheKey}.",
                    key);
            }
        }

        public async Task RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            ValidateKey(key);

            try
            {
                await _cache.RemoveAsync(
                    key,
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Cache removal failed for key {CacheKey}.",
                    key);
            }
        }

        private static void ValidateKey(
            string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(
                    "Cache key cannot be empty.",
                    nameof(key));
            }
        }
    }
}