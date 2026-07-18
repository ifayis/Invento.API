using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Products.Commands
{
    public class DeleteProductImageCommandHandler
        : ICommandHandler<
            DeleteProductImageCommand,
            ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly IFileStorageService _storage;
        private readonly ICacheVersionService _cacheVersion;

        public DeleteProductImageCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            IFileStorageService storage,
            ICacheVersionService cacheVersion)
        {
            _context = context;
            _currentTenant = currentTenant;
            _storage = storage;
            _cacheVersion = cacheVersion;
        }

        public async Task<ApiResponse<string>> Handle(
            DeleteProductImageCommand request,
            CancellationToken cancellationToken)
        {
            var image =
                await _context.ProductImages
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == request.ImageId
                            && x.TenantId ==
                                _currentTenant.TenantId
                            && !x.IsDeleted,
                        cancellationToken);

            if (image is null)
            {
                return ApiResponse<string>
                    .FailureResponse(
                        new()
                        {
                            "Image not found."
                        });
            }

            await _storage.DeleteFileAsync(
                image.ImageUrl,
                cancellationToken);

            image.IsDeleted = true;

            await _context.SaveChangesAsync(
                cancellationToken);

            await _cacheVersion
                .IncrementVersionAsync(
                    _currentTenant.TenantId,
                    "products",
                    cancellationToken);

            return ApiResponse<string>
                .SuccessResponse(
                    "Image deleted successfully.");
        }
    }
}