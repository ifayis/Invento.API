namespace Invento.Application.Interfaces
{
    public interface ICacheVersionService
    {
        Task<long> GetVersionAsync(
            Guid tenantId,
            string cacheGroup,
            CancellationToken cancellationToken = default);

        Task<long> IncrementVersionAsync(
            Guid tenantId,
            string cacheGroup,
            CancellationToken cancellationToken = default);
    }
}