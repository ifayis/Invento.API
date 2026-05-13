using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Products.Commands
{
    public class CreateProductCommandHandler
        : ICommandHandler<
            CreateProductCommand,
            ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;

        public CreateProductCommandHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<Guid>> Handle(
            CreateProductCommand request,
            CancellationToken cancellationToken)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(x =>
                    x.Id == request.CategoryId,
                    cancellationToken);

            if (!categoryExists)
            {
                return ApiResponse<Guid>
                    .FailureResponse(
                        new List<string>
                        {
                        "Category not found"
                        });
            }

            var product = new Product
            {
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

            return ApiResponse<Guid>
                .SuccessResponse(
                    product.Id,
                    "Product created successfully");
        }
    }
}