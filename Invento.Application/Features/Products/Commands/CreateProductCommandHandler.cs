using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Products.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Invento.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Invento.Application.Common.Services;

namespace Invento.Application.Features.Products.Commands
{
    public class CreateProductCommandHandler
        : ICommandHandler<CreateProductCommand, ApiResponse<ProductDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly StockMovementService _stockMovementService;

        public CreateProductCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            StockMovementService stockMovementService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _stockMovementService = stockMovementService;
        }

        public async Task<ApiResponse<ProductDto>> Handle(
            CreateProductCommand request,
            CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(x =>
                    x.Id == request.CategoryId
                    && x.TenantId == _currentTenant.TenantId
                    && !x.IsDeleted,
                    cancellationToken
                );

            if (category is null)
            {
                return ApiResponse<ProductDto>
                    .FailureResponse(
                        new List<string>
                        {
                            "Category not found"
                        }
                    );
            }

            var product = new Product
            {
                TenantId = _currentTenant.TenantId,
                Name = request.Name.Trim(),
                SKU = request.SKU.Trim(),
                CostPrice = 0,
                SellingPrice = request.SellingPrice,
                CurrentStock = 0,
                CategoryId = request.CategoryId,
                LowStockThreshold = request.LowStockThreshold
            };

            await _context.Products.AddAsync(
                product,
                cancellationToken
            );

            await _context.SaveChangesAsync(cancellationToken);

            var response = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                CostPrice = product.CostPrice,
                SellingPrice = product.SellingPrice,
                CurrentStock = product.CurrentStock,
                CategoryName = category.Name,
                CreatedAt = product.CreatedAt,
                LowStockThreshold = product.LowStockThreshold
            };

            return ApiResponse<ProductDto>
                .SuccessResponse(
                    response,
                    "Product created successfully"
                );
        }
    }
}