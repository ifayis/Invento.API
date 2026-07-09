using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Services;
using Invento.Application.Features.Purchases.DTOs;
using Invento.Application.Features.Purchases.Extensions;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Invento.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Purchases.Commands
{
    public class UpdatePurchaseCommandHandler
        : ICommandHandler<
            UpdatePurchaseCommand,
            ApiResponse<PurchaseDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly StockMovementService _stockMovementService;

        public UpdatePurchaseCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            StockMovementService stockMovementService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _stockMovementService = stockMovementService;
        }

        public async Task<ApiResponse<PurchaseDto>> Handle(
            UpdatePurchaseCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentTenant.TenantId;

            await using var transaction =
                await _context.BeginTransactionAsync(
                    cancellationToken);
            try
            {
                var purchase = await _context.Purchases
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

                    return ApiResponse<PurchaseDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Purchase not found"
                            });
                }

                if (request.Items is null
                    || request.Items.Count == 0)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return ApiResponse<PurchaseDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "At least one purchase item is required"
                            });
                }

                if (request.DiscountAmount < 0)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return ApiResponse<PurchaseDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Discount amount cannot be negative"
                            });
                }

                if (request.Items.Any(
                    x => x.Quantity <= 0))
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return ApiResponse<PurchaseDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "All item quantities must be greater than zero"
                            });
                }

                if (request.Items.Any(
                    x => x.UnitCost < 0))
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return ApiResponse<PurchaseDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Unit cost cannot be negative"
                            });
                }

                if (request.Items.Any(
                    x => x.TaxRate < 0))
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return ApiResponse<PurchaseDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Tax rate cannot be negative"
                            });
                }

                var supplierExists =
                    await _context.Suppliers
                        .AnyAsync(
                            x =>
                                x.Id == request.SupplierId
                                && x.TenantId == tenantId
                                && !x.IsDeleted,
                            cancellationToken);

                if (!supplierExists)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return ApiResponse<PurchaseDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Supplier not found"
                            });
                }

                var duplicateProductIds =
                    request.Items
                        .GroupBy(x => x.ProductId)
                        .Where(x => x.Count() > 1)
                        .Select(x => x.Key)
                        .ToList();

                if (duplicateProductIds.Count > 0)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return ApiResponse<PurchaseDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Duplicate products are not allowed " +
                                "in the same purchase request"
                            });
                }

                var oldItems =
                    purchase.PurchaseItems.ToList();

                var requiredProductIds =
                    oldItems
                        .Select(x => x.ProductId)
                        .Concat(
                            request.Items.Select(
                                x => x.ProductId))
                        .Distinct()
                        .ToList();

                var products =
                    await _context.Products
                        .Where(
                            x =>
                                requiredProductIds.Contains(x.Id)
                                && x.TenantId == tenantId)
                        .ToListAsync(cancellationToken);

                var productById =
                    products.ToDictionary(
                        x => x.Id);

                var missingNewProductIds =
                    request.Items
                        .Select(x => x.ProductId)
                        .Distinct()
                        .Where(
                            productId =>
                                !productById.TryGetValue(
                                    productId,
                                    out var product)
                                || product.IsDeleted)
                        .ToList();

                if (missingNewProductIds.Count > 0)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return ApiResponse<PurchaseDto>
                        .FailureResponse(
                            missingNewProductIds
                                .Select(
                                    id =>
                                        $"Product not found: {id}")
                                .ToList());
                }

                var missingOldProductIds =
                    oldItems
                        .Select(x => x.ProductId)
                        .Distinct()
                        .Where(
                            productId =>
                                !productById.ContainsKey(productId))
                        .ToList();

                if (missingOldProductIds.Count > 0)
                {
                    throw new InvalidOperationException(
                        "Existing purchase references product records " +
                        "that could not be loaded for this tenant.");
                }

                var stockAfterReversal =
                    products.ToDictionary(
                        x => x.Id,
                        x => x.CurrentStock);

                foreach (var oldItem in oldItems)
                {
                    var currentStock =
                        stockAfterReversal[
                            oldItem.ProductId];

                    if (currentStock < oldItem.Quantity)
                    {
                        var product =
                            productById[
                                oldItem.ProductId];

                        await transaction.RollbackAsync(
                            cancellationToken);

                        return ApiResponse<PurchaseDto>
                            .FailureResponse(
                                new List<string>
                                {
                                    $"Cannot update purchase. " +
                                    $"Product '{product.Name}' stock " +
                                    $"already consumed."
                                });
                    }

                    stockAfterReversal[
                        oldItem.ProductId] -=
                        oldItem.Quantity;
                }

                foreach (var oldItem in oldItems)
                {
                    var product =
                        productById[
                            oldItem.ProductId];

                    product.CurrentStock -=
                        oldItem.Quantity;

                    await _stockMovementService.CreateMovement(
                        product.Id,
                        oldItem.Quantity,
                        StockMovementType.PurchaseReturn.ToString(),
                        product.CurrentStock,
                        "Purchase update reversal",
                        purchase.PurchaseNumber,
                        cancellationToken);
                }

                _context.PurchaseItems.RemoveRange(
                    oldItems);

                await _context.SaveChangesAsync(
                    cancellationToken);

                purchase.PurchaseItems.Clear();

                decimal subTotal = 0;
                decimal totalTax = 0;

                purchase.SupplierId =
                    request.SupplierId;

                purchase.PurchaseDate =
                    request.PurchaseDate;

                purchase.DiscountAmount =
                    request.DiscountAmount;

                foreach (var item in request.Items)
                {
                    var product =
                        productById[
                            item.ProductId];

                    product.CurrentStock +=
                        item.Quantity;

                    product.CostPrice =
                        item.UnitCost;

                    var itemSubTotal =
                        item.UnitCost
                        * item.Quantity;

                    var taxAmount =
                        (itemSubTotal
                        * item.TaxRate)
                        / 100;

                    var totalPrice =
                        itemSubTotal
                        + taxAmount;

                    var purchaseItem =
                        new PurchaseItem
                        {
                            TenantId = tenantId,
                            PurchaseId = purchase.Id,
                            ProductId = product.Id,
                            Quantity = item.Quantity,
                            UnitCost = item.UnitCost,
                            TaxRate = item.TaxRate,
                            TaxAmount = taxAmount,
                            TotalPrice = totalPrice
                        };

                    await _context.PurchaseItems
                        .AddAsync(
                            purchaseItem,
                            cancellationToken);

                    await _stockMovementService.CreateMovement(
                        product.Id,
                        item.Quantity,
                        StockMovementType.Purchase.ToString(),
                        product.CurrentStock,
                        "Purchase updated",
                        purchase.PurchaseNumber,
                        cancellationToken);

                    subTotal += itemSubTotal;
                    totalTax += taxAmount;
                }

                var totalAmount =
                    subTotal
                    + totalTax
                    - request.DiscountAmount;

                if (totalAmount < 0)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return ApiResponse<PurchaseDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Discount amount cannot exceed " +
                                "the purchase total"
                            });
                }

                purchase.SubTotal =
                    subTotal;

                purchase.TaxAmount =
                    totalTax;

                purchase.TotalAmount =
                    totalAmount;

                purchase.DueAmount =
                    purchase.TotalAmount
                    - purchase.PaidAmount;

                purchase.PaymentStatus =
                    purchase.DueAmount <= 0
                        ? PaymentStatus.Paid
                        : purchase.PaidAmount > 0
                            ? PaymentStatus.PartiallyPaid
                            : PaymentStatus.Unpaid;

                await _context.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);

                return ApiResponse<PurchaseDto>
                    .SuccessResponse(
                        purchase.ToPurchaseDto(),
                        "Purchase updated successfully");
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
                "Stock changed while the purchase was being updated. " +
                "Please reload the latest data and try again."
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