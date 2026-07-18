using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Products.Commands
{
    public class UploadProductImageCommandHandler
        : ICommandHandler<
            UploadProductImageCommand,
            ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly ICurrentUserService _currentUser;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICacheVersionService _cacheVersionService;

        public UploadProductImageCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            ICurrentUserService currentUser,
            IFileStorageService fileStorageService,
            ICacheVersionService cacheVersionService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _currentUser = currentUser;
            _fileStorageService = fileStorageService;
            _cacheVersionService = cacheVersionService;
        }

        public async Task<ApiResponse<Guid>> Handle(
            UploadProductImageCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId =
                _currentTenant.TenantId;

            var product =
                await _context.Products
                    .Include(x => x.Images)
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == request.ProductId
                            && x.TenantId == tenantId
                            && !x.IsDeleted,
                        cancellationToken);

            if (product is null)
            {
                return ApiResponse<Guid>
                    .FailureResponse(
                        new()
                        {
                            "Product not found."
                        });
            }

            var relativePath =
                await _fileStorageService
                    .SaveProductImageAsync(
                        tenantId,
                        product.Id,
                        request.Image,
                        cancellationToken);

            var image =
                new ProductImage
                {
                    TenantId = tenantId,

                    ProductId = product.Id,

                    FileName =
                        Path.GetFileName(relativePath),

                    OriginalFileName =
                        request.Image.FileName,

                    ContentType =
                        request.Image.ContentType,

                    FileSize =
                        request.Image.Length,

                    ImageUrl =
                        relativePath,

                    IsPrimary =
                        !product.Images.Any(),
                };

            var imageCount =
                await _context.ProductImages
                    .CountAsync(
                        x =>
                            x.ProductId == product.Id
                            && x.TenantId == tenantId
                            && !x.IsDeleted,
                        cancellationToken);

            if (imageCount >= 10)
            {
                return ApiResponse<Guid>
                    .FailureResponse(
                        new List<string>
                        {
                            "A product can have a maximum of 10 images."
                        });
            }

            await _context.ProductImages
                .AddAsync(
                    image,
                    cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);

            await _cacheVersionService
                .IncrementVersionAsync(
                    tenantId,
                    CacheGroups.Products,
                    cancellationToken);

            return ApiResponse<Guid>
                .SuccessResponse(
                    image.Id,
                    "Image uploaded successfully.");
        }
    }
}