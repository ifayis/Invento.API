using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Products.DTOs;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Products.Commands
{
    public class UpdateProductCommandHandler
        : ICommandHandler<
            UpdateProductCommand,
            ApiResponse<ProductDto>>
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

        public async Task<ApiResponse<ProductDto>> Handle(
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
                return ApiResponse<ProductDto>
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

            return ApiResponse<ProductDto>
                .SuccessResponse(
                    new ProductDto
                    {
                        Name = product.Name,
                        SKU = product.SKU,
                        CostPrice = product.CostPrice,
                        SellingPrice = product.SellingPrice
                    },
                    "Product updated successfully");
        }
    }
}