using Invento.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Invento.Infrastructure.Storage
{
    public class LocalFileStorageService
        : IFileStorageService
    {
        private static readonly string[]
            AllowedExtensions =
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

        private const long
            MaxFileSize =
                5 * 1024 * 1024;

        private readonly IWebHostEnvironment
            _environment;

        public LocalFileStorageService(
            IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveProductImageAsync(
            Guid tenantId,
            Guid productId,
            IFormFile file,
            CancellationToken cancellationToken = default)
        {
            if (file is null || file.Length == 0)
            {
                throw new InvalidOperationException(
                    "Image file is required.");
            }

            if (file.Length > MaxFileSize)
            {
                throw new InvalidOperationException(
                    "Maximum file size is 5 MB.");
            }

            var extension =
                Path.GetExtension(file.FileName)
                    .ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException(
                    "Only jpg, jpeg, png and webp images are allowed.");
            }

            var uploadsRoot =
                Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "products",
                    tenantId.ToString(),
                    productId.ToString());

            Directory.CreateDirectory(
                uploadsRoot);

            var fileName =
                $"{Guid.NewGuid()}{extension}";

            var fullPath =
                Path.Combine(
                    uploadsRoot,
                    fileName);

            await using var stream =
                new FileStream(
                    fullPath,
                    FileMode.Create);

            await file.CopyToAsync(
                stream,
                cancellationToken);

            return Path.Combine(
                    "uploads",
                    "products",
                    tenantId.ToString(),
                    productId.ToString(),
                    fileName)
                .Replace("\\", "/");
        }

        public Task DeleteFileAsync(
            string relativePath,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return Task.CompletedTask;
            }

            var fullPath =
                Path.Combine(
                    _environment.WebRootPath,
                    relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }
    }
}