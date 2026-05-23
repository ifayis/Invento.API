using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Products.DTOs;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Products.Commands
{
    public class RestoreProductCommandHandler
        : ICommandHandler<RestoreProductCommand, ApiResponse<ProductStatusDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;

        public RestoreProductCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant)
        {
            _context = context;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<ProductStatusDto>> Handle(
            RestoreProductCommand request,
            CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x =>
                    x.Id == request.Id
                    && x.TenantId ==
                    _currentTenant.TenantId
                    && x.IsDeleted,
                    cancellationToken
                );

            if (product is null)
            {
                return ApiResponse<ProductStatusDto>
                    .FailureResponse(
                        new List<string>
                        {
                            "Hidden product not found"
                        }
                    );
            }

            product.IsDeleted = false;

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<ProductStatusDto>
                .SuccessResponse(
                    new ProductStatusDto
                    {
                        Id = product.Id,
                        Name = product.Name
                    },
                    "Product restored successfully"
                );
        }
    }
}