using Invento.Application.Interfaces;
using StackExchange.Redis;

namespace Invento.Infrastructure.Caching
{
    public class RedisCacheVersionService
        : ICacheVersionService
    {
        private readonly IDatabase _database;

        public RedisCacheVersionService(
            IConnectionMultiplexer connectionMultiplexer)
        {
            _database =
                connectionMultiplexer.GetDatabase();
        }

        private static string BuildKey(
            Guid tenantId,
            string cacheGroup)
        {
            return
                $"cache-version:{tenantId:N}:{cacheGroup}";
        }

        public async Task<long> GetVersionAsync(
            Guid tenantId,
            string cacheGroup,
            CancellationToken cancellationToken = default)
        {
            var key =
                BuildKey(
                    tenantId,
                    cacheGroup);

            var value =
                await _database.StringGetAsync(key);

            if (!value.HasValue)
            {
                await _database.StringSetAsync(
                    key,
                    1);

                return 1;
            }

            return (long)value;
        }
        public async Task<long> IncrementVersionAsync(
            Guid tenantId,
            string cacheGroup,
            CancellationToken cancellationToken = default)
        {
            return await _database.StringIncrementAsync(
                BuildKey(
                    tenantId,
                    cacheGroup));
        }
    }
}