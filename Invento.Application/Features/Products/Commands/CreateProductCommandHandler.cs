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
            IApplicationDbContext context, ICurrentTenantService currentTenant)
        {
            _context = context;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<ProductDto>> Handle(
            CreateProductCommand request,
            CancellationToken cancellationToken)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(x =>
                    x.Id == request.CategoryId
                    && x.TenantId == _currentTenant.TenantId,
                    cancellationToken);

            if (!categoryExists)
            {
                return ApiResponse<ProductDto>
                    .FailureResponse(
                        new List<string>
                        {
                        "Category not found"
                        });
            }

            var product = new Product
            {
                TenantId = _currentTenant.TenantId,
                Name = request.Name,
                SKU = request.SKU,
                CostPrice = request.CostPrice,
                SellingPrice = request.SellingPrice,
                CurrentStock = request.CurrentStock,
                CategoryId = request.CategoryId
            };

            await _context.Products.AddAsync(
                product,
                cancellationToken);

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
                    "Product created successfully");
        }
    }
}