public interface ICacheableQuery
{
    string GetCacheKey();

    TimeSpan Expiration { get; }
}