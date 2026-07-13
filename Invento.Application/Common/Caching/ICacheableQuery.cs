namespace Invento.Application.Common.Caching
{
    public interface ICacheableQuery
    {
        TimeSpan Expiration { get; }

        string CacheGroup { get; }

        string GetCacheKey();
    }
}