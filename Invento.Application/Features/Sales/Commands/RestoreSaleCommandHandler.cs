using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Services;
using Invento.Application.Features.Sales.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Invento.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Sales.Commands
{
    public class RestoreSaleCommandHandler
        : ICommandHandler<RestoreSaleCommand, ApiResponse<DeleteSaleDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly StockMovementService _stockMovementService;


        public RestoreSaleCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            StockMovementService stockMovementService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _stockMovementService = stockMovementService;
        }

        public async Task<ApiResponse<DeleteSaleDto>> Handle(
            RestoreSaleCommand request,
            CancellationToken cancellationToken)
        {
            var sale = await _context.Sales
                .IgnoreQueryFilters()
                .Include(x => x.SaleItems)
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id
                    && x.TenantId == _currentTenant.TenantId
                    && x.IsDeleted,
                    cancellationToken
                );

            if (sale is null)
            {
                return ApiResponse<DeleteSaleDto>
                    .FailureResponse(
                        new()
                        {
                            "Hidden sale not found"
                        }
                    );
            }

            foreach (var item in sale.SaleItems)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(
                        x => x.Id == item.ProductId
                        && x.TenantId == _currentTenant.TenantId,
                        cancellationToken);

                if (product is null)
                {
                    return ApiResponse<DeleteSaleDto>
                        .FailureResponse(
                            new()
                            {
                    $"Product not found : {item.ProductId}"
                            });
                }

                if (product.CurrentStock < item.Quantity)
                {
                    return ApiResponse<DeleteSaleDto>
                        .FailureResponse(
                            new()
                            {
                    $"Insufficient stock to restore sale for product {product.Name}"
                            });
                }

                product.CurrentStock -= item.Quantity;

                await _stockMovementService.CreateMovement(
                    product.Id,
                    item.Quantity,
                    StockMovementType.Sale.ToString(),
                    product.CurrentStock,
                    "Sale restored",
                    sale.InvoiceNumber
                );
            }

            sale.IsDeleted = false;

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<DeleteSaleDto>
                .SuccessResponse(
                    new DeleteSaleDto
                    {
                        Id = sale.Id,
                        InvoiceNumber = sale.InvoiceNumber,
                        IsDeleted = false
                    },
                    "Sale restored successfully"
                );
        }
    }
}