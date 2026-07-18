using Microsoft.AspNetCore.Http;

namespace Invento.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveProductImageAsync(
            Guid tenantId,
            Guid productId,
            IFormFile file,
            CancellationToken cancellationToken = default);

        Task DeleteFileAsync(
            string relativePath,
            CancellationToken cancellationToken = default);
    }
}