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
            var tenantId = _currentTenant.TenantId;

            await using var transaction =
                await _context.BeginTransactionAsync(
                    cancellationToken);

            try
            {
                var purchase =
                    await _context.Purchases
                        .Include(x => x.Supplier)
                        .Include(x => x.PurchaseItems)
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id == request.Id
                                && x.TenantId == tenantId
                                && !x.IsDeleted,
                            cancellationToken);

                if (purchase is null)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    _context.ClearChangeTracker();

                    return ApiResponse<PurchaseDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Purchase not found"
                            });
                }

                var purchaseItems =
                    purchase.PurchaseItems.ToList();

                var productIds =
                    purchaseItems
                        .Select(x => x.ProductId)
                        .Distinct()
                        .ToList();

                var products =
                    await _context.Products
                        .Where(
                            x =>
                                productIds.Contains(x.Id)
                                && x.TenantId == tenantId)
                        .ToListAsync(cancellationToken);

                var productById =
                    products.ToDictionary(
                        x => x.Id);

                var missingProductIds =
                    productIds
                        .Where(
                            productId =>
                                !productById.ContainsKey(productId))
                        .ToList();

                if (missingProductIds.Count > 0)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    _context.ClearChangeTracker();

                    return ApiResponse<PurchaseDto>
                        .FailureResponse(
                            missingProductIds
                                .Select(
                                    id =>
                                        $"Product not found: {id}")
                                .ToList());
                }

                var requiredQuantityByProductId =
                    purchaseItems
                        .GroupBy(x => x.ProductId)
                        .ToDictionary(
                            group => group.Key,
                            group => group.Sum(
                                item => item.Quantity));

                foreach (var requiredStock in
                    requiredQuantityByProductId)
                {
                    var product =
                        productById[
                            requiredStock.Key];

                    if (product.CurrentStock <
                        requiredStock.Value)
                    {
                        await transaction.RollbackAsync(
                            cancellationToken);

                        _context.ClearChangeTracker();

                        return ApiResponse<PurchaseDto>
                            .FailureResponse(
                                new List<string>
                                {
                                    $"Cannot delete purchase. " +
                                    $"Product '{product.Name}' stock " +
                                    $"has already been consumed. " +
                                    $"Current stock: " +
                                    $"{product.CurrentStock}, " +
                                    $"required for reversal: " +
                                    $"{requiredStock.Value}."
                                });
                    }
                }

                foreach (var item in purchaseItems)
                {
                    var product =
                        productById[item.ProductId];

                    product.CurrentStock -=
                        item.Quantity;

                    await _stockMovementService
                        .CreateMovement(
                            product.Id,
                            item.Quantity,
                            StockMovementType
                                .PurchaseReturn
                                .ToString(),
                            product.CurrentStock,
                            "Purchase deleted",
                            purchase.PurchaseNumber,
                            cancellationToken);
                }

                purchase.IsDeleted = true;

                await _context.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);

                return ApiResponse<PurchaseDto>
                    .SuccessResponse(
                        purchase.ToPurchaseDto(),
                        "Purchase deleted successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(
                    CancellationToken.None);

                _context.ClearChangeTracker();

                return ApiResponse<PurchaseDto>
                    .FailureResponse(
                        new List<string>
                        {
                            "Stock changed while the purchase " +
                            "was being deleted. Reload the latest " +
                            "data and try again."
                        },
                        "Concurrency conflict");
            }
            catch
            {
                await transaction.RollbackAsync(
                    CancellationToken.None);

                _context.ClearChangeTracker();

                throw;
            }
        }
    }
}