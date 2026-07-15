using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Common.Extensions;
using Invento.Application.Features.Products.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Products.Commands
{
    public class DeleteProductCommandHandler
        : ICommandHandler<DeleteProductCommand, ApiResponse<ProductStatusDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly ICacheVersionService _cacheVersionService;

        public DeleteProductCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            ICacheVersionService cacheVersionService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _cacheVersionService = cacheVersionService;
        }

        public async Task<ApiResponse<ProductStatusDto>> Handle(
            DeleteProductCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentTenant.TenantId;

            var product = await _context.Products
                .FirstOrDefaultAsync(x =>
                    x.Id == request.Id
                    && x.TenantId ==
                    _currentTenant.TenantId
                    && !x.IsDeleted,
                    cancellationToken
                );

            if (product is null)
            {
                return ApiResponse<ProductStatusDto>
                    .FailureResponse(
                        new List<string>
                        {
                            "Product not found"
                        }
                    );
            }

            product.IsDeleted = true;

            await _context.SaveChangesAsync(cancellationToken);

            await _cacheVersionService.InvalidateAsync(
                    tenantId,
                    CacheGroups.Products,
                    CacheGroups.Reports,
                    CacheGroups.Dashboard);

            return ApiResponse<ProductStatusDto>
                .SuccessResponse(
                    new ProductStatusDto
                    {
                        Id = product.Id,
                        Name = product.Name
                    },
                    "Product hidden successfully"
                );
        }
    }
}