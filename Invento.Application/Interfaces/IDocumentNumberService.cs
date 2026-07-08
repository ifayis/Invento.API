namespace Invento.Application.Interfaces
{
    public interface IDocumentNumberService
    {
        Task<string> GenerateNextAsync(
            Guid tenantId,
            string documentType,
            string prefix,
            DateTime documentDate,
            CancellationToken cancellationToken = default);
    }
}