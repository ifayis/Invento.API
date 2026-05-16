using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Products.Commands
{
    public class UpdateProductCommandHandler
        : ICommandHandler<
            UpdateProductCommand,
            ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;

        public UpdateProductCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant)
        {
            _context = context;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<Guid>> Handle(
            UpdateProductCommand request,
            CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x =>
                x.Id == request.Id
                && x.TenantId == _currentTenant.TenantId 
                && !x.IsDeleted,
                cancellationToken);

            if (product is null)
            {
                return ApiResponse<Guid>
                    .FailureResponse(
                        new List<string>
                        {
                        "Product not found"
                        });
            }
            
            product.Name = request.Name;
            product.SKU = request.SKU;
            product.CostPrice = request.CostPrice;
            product.SellingPrice = request.SellingPrice;
            product.CurrentStock = request.CurrentStock;
            product.CategoryId = request.CategoryId;

            await _context.SaveChangesAsync(
                cancellationToken);

            return ApiResponse<Guid>
                .SuccessResponse(
                    product.Id,
                    "Product updated successfully");
        }
    }
}