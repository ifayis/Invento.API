using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Products.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Products.Commands
{
    public class CreateProductCommandHandler
        : ICommandHandler<
            CreateProductCommand,
            ApiResponse<ProductDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;

        public CreateProductCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant)
        {
            _context = context;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<ProductDto>> Handle(
            CreateProductCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId =
                _currentTenant.TenantId;

            var category =
                await _context.Categories
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == request.CategoryId
                            && x.TenantId == tenantId
                            && !x.IsDeleted,
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

            var product =
                new Product
                {
                    TenantId = tenantId,
                    Name = request.Name.Trim(),
                    SKU = request.SKU.Trim(),
                    CostPrice = 0,
                    SellingPrice = request.SellingPrice,
                    CurrentStock = 0,
                    CategoryId = request.CategoryId,
                    LowStockThreshold =
                        request.LowStockThreshold,
                    CriticalStockThreshold =
                        request.CriticalStockThreshold
                };

            await _context.Products.AddAsync(
                product,
                cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);

            var response =
                new ProductDto
                {
                    Id = product.Id,
                    CategoryId = product.CategoryId,
                    Name = product.Name,
                    SKU = product.SKU,
                    CostPrice = product.CostPrice,
                    SellingPrice = product.SellingPrice,
                    CurrentStock = product.CurrentStock,
                    CategoryName = category.Name,
                    IsDeleted = product.IsDeleted,
                    CreatedAt = product.CreatedAt,
                    LowStockThreshold =
                        product.LowStockThreshold,
                    CriticalStockThreshold =
                        product.CriticalStockThreshold
                };

            return ApiResponse<ProductDto>
                .SuccessResponse(
                    response,
                    "Product created successfully");
        }
    }
}