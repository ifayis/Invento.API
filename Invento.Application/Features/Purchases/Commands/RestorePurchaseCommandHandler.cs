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
    public class RestorePurchaseCommandHandler
        : ICommandHandler<
            RestorePurchaseCommand,
            ApiResponse<PurchaseDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly StockMovementService _stockMovementService;

        public RestorePurchaseCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            StockMovementService stockMovementService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _stockMovementService = stockMovementService;
        }

        public async Task<ApiResponse<PurchaseDto>> Handle(
            RestorePurchaseCommand request,
            CancellationToken cancellationToken)
        {
            using var transaction =
                await _context.BeginTransactionAsync();

            try
            {
                var purchase =
                    await _context.Purchases
                    .IgnoreQueryFilters()
                    .Include(x => x.Supplier)
                    .Include(x => x.PurchaseItems)
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == request.Id
                            && x.TenantId ==
                                _currentTenant.TenantId
                            && x.IsDeleted,
                        cancellationToken);

                if (purchase is null)
                {
                    return ApiResponse<PurchaseDto>
                        .FailureResponse(
                            ["Purchase not found"]);
                }

                foreach (var item in purchase.PurchaseItems)
                {
                    var product =
                        await _context.Products
                        .FirstAsync(
                            x =>
                                x.Id == item.ProductId
                                && x.TenantId ==
                                    _currentTenant.TenantId,
                            cancellationToken);

                    product.CurrentStock += item.Quantity;

                    product.CostPrice = item.UnitCost;

                    await _stockMovementService.CreateMovement(
                        product.Id,
                        item.Quantity,
                        StockMovementType.Purchase.ToString(),
                        product.CurrentStock,
                        "Purchase restored",
                        purchase.PurchaseNumber);
                }

                purchase.IsDeleted = false;

                await _context.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);

                return ApiResponse<PurchaseDto>
                    .SuccessResponse(
                        purchase.ToPurchaseDto(),
                        "Purchase restored successfully");
            }
            catch
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                throw;
            }
        }
    }
}