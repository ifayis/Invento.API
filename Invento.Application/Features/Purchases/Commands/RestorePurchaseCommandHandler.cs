using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Common.Extensions;
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
        private readonly ICacheVersionService _cacheVersionService;

        public RestorePurchaseCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            StockMovementService stockMovementService,
            ICacheVersionService cacheVersionService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _stockMovementService = stockMovementService;
            _cacheVersionService = cacheVersionService;
        }

        public async Task<ApiResponse<PurchaseDto>> Handle(
            RestorePurchaseCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentTenant.TenantId;

            var strategy =
                _context.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(
                async () =>
                {

                    await using var transaction =
                await _context.BeginTransactionAsync(
                    cancellationToken);

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
                                        && x.TenantId == tenantId
                                        && x.IsDeleted,
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
                                        && x.TenantId == tenantId
                                        && !x.IsDeleted)
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
                                                $"Product not found or inactive: {id}")
                                        .ToList());
                        }

                        foreach (var item in purchaseItems)
                        {
                            var product =
                                productById[item.ProductId];

                            product.CurrentStock +=
                                item.Quantity;

                            product.CostPrice =
                                item.UnitCost;

                            await _stockMovementService
                                .CreateMovement(
                                    product.Id,
                                    item.Quantity,
                                    StockMovementType
                                        .Purchase
                                        .ToString(),
                                    product.CurrentStock,
                                    "Purchase restored",
                                    purchase.PurchaseNumber,
                                    cancellationToken);
                        }

                        purchase.IsDeleted = false;

                        await _context.SaveChangesAsync(
                            cancellationToken);

                        await _cacheVersionService.InvalidateAsync(
                                tenantId,
                                CacheGroups.Purchases,
                                CacheGroups.Payables,
                                CacheGroups.Balance,
                                CacheGroups.Products,
                                CacheGroups.Reports,
                                CacheGroups.Dashboard);

                        await transaction.CommitAsync(
                            cancellationToken);

                        return ApiResponse<PurchaseDto>
                            .SuccessResponse(
                                purchase.ToPurchaseDto(),
                                "Purchase restored successfully");
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
                            "was being restored. Reload the latest " +
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
            );
        }
    }
}