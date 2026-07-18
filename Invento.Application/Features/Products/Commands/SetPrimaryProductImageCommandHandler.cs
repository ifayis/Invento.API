using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Products.Commands
{
    public class SetPrimaryProductImageCommandHandler
        : ICommandHandler<
            SetPrimaryProductImageCommand,
            ApiResponse<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly ICacheVersionService _cacheVersionService;

        public SetPrimaryProductImageCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            ICacheVersionService cacheVersionService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _cacheVersionService = cacheVersionService;
        }

        public async Task<ApiResponse<string>> Handle(
            SetPrimaryProductImageCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId =
                _currentTenant.TenantId;

            var image =
                await _context.ProductImages
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == request.ImageId
                            && x.TenantId == tenantId
                            && !x.IsDeleted,
                        cancellationToken);

            if (image is null)
            {
                return ApiResponse<string>
                    .FailureResponse(
                        new List<string>
                        {
                            "Image not found."
                        });
            }

            var images =
                await _context.ProductImages
                    .Where(
                        x =>
                            x.ProductId == image.ProductId
                            && x.TenantId == tenantId
                            && !x.IsDeleted)
                    .ToListAsync(
                        cancellationToken);

            foreach (var productImage in images)
            {
                productImage.IsPrimary = false;
            }

            image.IsPrimary = true;

            await _context.SaveChangesAsync(
                cancellationToken);

            await _cacheVersionService
                .IncrementVersionAsync(
                    tenantId,
                    CacheGroups.Products,
                    cancellationToken);

            return ApiResponse<string>
                .SuccessResponse(
                    "Primary image updated successfully.");
        }
    }
}