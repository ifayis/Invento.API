using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Services;
using Invento.Application.Features.Purchases.DTOs;
using Invento.Application.Features.Purchases.Extensions;
using Invento.Application.Interfaces;
using Invento.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Purchases.Commands
{
    public class DeletePurchaseCommandHandler
        : ICommandHandler<
            DeletePurchaseCommand,
            ApiResponse<PurchaseDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly StockMovementService _stockMovementService;

        public DeletePurchaseCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            StockMovementService stockMovementService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _stockMovementService = stockMovementService;
        }

        public async Task<ApiResponse<PurchaseDto>> Handle(
            DeletePurchaseCommand request,
            CancellationToken cancellationToken)
        {
            using var transaction =
                await _context.BeginTransactionAsync();

            try
            {
                var purchase = await _context.Purchases
                    .Include(x => x.Supplier)
                    .Include(x => x.PurchaseItems)
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == request.Id
                            && x.TenantId == _currentTenant.TenantId
                            && !x.IsDeleted,
                        cancellationToken);

                if (purchase is null)
                {
                    return ApiResponse<PurchaseDto>
                        .FailureResponse(
                            ["Purchase not found"]);
                }

                foreach (var item in purchase.PurchaseItems)
                {
                    var product = await _context.Products
                        .FirstAsync(
                            x =>
                                x.Id == item.ProductId
                                && x.TenantId == _currentTenant.TenantId,
                            cancellationToken);

                    if (product.CurrentStock < item.Quantity)
                    {
                        return ApiResponse<PurchaseDto>
                            .FailureResponse(
                                [$"Cannot delete purchase. Product '{product.Name}' stock already consumed."]);
                    }

                    product.CurrentStock -= item.Quantity;

                    await _stockMovementService.CreateMovement(
                        product.Id,
                        item.Quantity,
                        StockMovementType.PurchaseReturn.ToString(),
                        product.CurrentStock,
                        "Purchase deleted",
                        purchase.PurchaseNumber);
                }

                purchase.IsDeleted = true;

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return ApiResponse<PurchaseDto>
                    .SuccessResponse(
                        purchase.ToPurchaseDto(),
                        "Purchase deleted successfully");
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}