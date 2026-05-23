using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Products.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Products.Commands
{
    public class UpdateProductCommandHandler
        : ICommandHandler< UpdateProductCommand, ApiResponse<ProductDto>>
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
                        }
                    );
            }

            var category = await _context.Categories
                           .FirstOrDefaultAsync(x =>
                               x.Id == request.CategoryId &&
                               x.TenantId == _currentTenant.TenantId &&
                               !x.IsDeleted,
                               cancellationToken);

            if (category is null)
            {
                return ApiResponse<ProductDto>
                    .FailureResponse(
                        new List<string>
                        {
                            "Category not found"
                        });
            }

            product.Name = request.Name;
            product.SKU = request.SKU;
            product.CostPrice = request.CostPrice;
            product.SellingPrice = request.SellingPrice;
            product.CurrentStock = request.CurrentStock;
            product.CategoryId = request.CategoryId;
            product.IsDeleted = request.IsDeleted;

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<ProductDto>
                .SuccessResponse(
                    new ProductDto
                    {
                        Id = product.Id,

                        Name = product.Name,

                        SKU = product.SKU,

                        CostPrice = product.CostPrice,

                        SellingPrice = product.SellingPrice,

                        CurrentStock = product.CurrentStock,

                        CategoryName = category.Name,

                        IsDeleted = product.IsDeleted,

                        CreatedAt = product.CreatedAt
                    },
                    "Product updated successfully"
                );
        }
    }
}