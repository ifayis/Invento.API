using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Services;
using Invento.Application.Features.Sales.Command;
using Invento.Application.Features.Sales.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Sales.Commands
{
    public class DeleteSaleCommandHandler
        : ICommandHandler<DeleteSaleCommand, ApiResponse<DeleteSaleDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly StockMovementService _stockMovementService; 

        public DeleteSaleCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            StockMovementService stockMovementService
            )
        {
            _context = context;
            _currentTenant = currentTenant;
            _stockMovementService = stockMovementService;
        }

        public async Task<ApiResponse<DeleteSaleDto>> Handle(
            DeleteSaleCommand request,
            CancellationToken cancellationToken)
        {
            using var transaction = await _context.BeginTransactionAsync();

            try
            {
                var sale = await _context.Sales
                    .Include(x => x.SaleItems)
                    .FirstOrDefaultAsync(x =>
                        x.Id == request.Id
                        && x.TenantId == _currentTenant.TenantId
                        && !x.IsDeleted,
                        cancellationToken);

                if (sale is null)
                {
                    return ApiResponse<DeleteSaleDto>
                        .FailureResponse(
                            new List<string>
                            {
                            "Sale not found"
                            }
                        );
                }

                foreach (var item in sale.SaleItems)
                {
                    var product = await _context.Products
                        .FirstAsync(x =>
                            x.Id == item.ProductId
                            && x.TenantId == _currentTenant.TenantId,
                            cancellationToken
                        );

                    product.CurrentStock += item.Quantity;

                    await _stockMovementService.CreateMovement(
                        product.Id,
                        item.Quantity,
                        StockMovementType.SaleRestore.ToString(),
                        product.CurrentStock,
                        "Sale deleted",
                        sale.InvoiceNumber
                    );
                }

                sale.IsDeleted = true;

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return ApiResponse<DeleteSaleDto>
                    .SuccessResponse(
                    new DeleteSaleDto
                    {
                        Id = sale.Id,
                        InvoiceNumber = sale.InvoiceNumber,
                        IsDeleted = true
                    },
                    "Sale deleted successfully");
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);

                throw;
            }
        }
    }
}