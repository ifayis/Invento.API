using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Services;
using Invento.Application.Features.Sales.Command;
using Invento.Application.Features.Sales.DTOs;
using Invento.Application.Features.Sales.Extensions;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Invento.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Sales.Commands
{
    public class UpdateSaleCommandHandler
        : ICommandHandler<
            UpdateSaleCommand,
            ApiResponse<SaleDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly StockMovementService _stockMovementService;

        public UpdateSaleCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            StockMovementService stockMovementService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _stockMovementService = stockMovementService;
        }

        public async Task<ApiResponse<SaleDto>> Handle(
            UpdateSaleCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentTenant.TenantId;

            await using var transaction =
                await _context.BeginTransactionAsync(
                    cancellationToken);
            try
            {
                var sale = await _context.Sales
                    .Include(x => x.SaleItems)
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == request.Id
                            && x.TenantId == tenantId
                            && !x.IsDeleted,
                        cancellationToken);

                if (sale is null)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return ApiResponse<SaleDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Sale not found"
                            });
                }

                if (request.Items is null
                    || request.Items.Count == 0)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return ApiResponse<SaleDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "At least one sale item is required"
                            });
                }

                if (request.DiscountAmount < 0)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return ApiResponse<SaleDto>
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

                    return ApiResponse<SaleDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "All item quantities must be greater than zero"
                            });
                }

                if (request.CustomerId.HasValue)
                {
                    var customerExists =
                        await _context.Customers
                            .AnyAsync(
                                x =>
                                    x.Id == request.CustomerId.Value
                                    && x.TenantId == tenantId
                                    && !x.IsDeleted,
                                cancellationToken);

                    if (!customerExists)
                    {
                        await transaction.RollbackAsync(
                            cancellationToken);

                        return ApiResponse<SaleDto>
                            .FailureResponse(
                                new List<string>
                                {
                                    "Customer not found"
                                });
                    }
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

                    return ApiResponse<SaleDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Duplicate products are not allowed " +
                                "in the same sale request"
                            });
                }

                var oldItems =
                    sale.SaleItems.ToList();

                var oldProductIds =
                    oldItems
                        .Select(x => x.ProductId);

                var newProductIds =
                    request.Items
                        .Select(x => x.ProductId);

                var requiredProductIds =
                    oldProductIds
                        .Concat(newProductIds)
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

                    return ApiResponse<SaleDto>
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
                        "Existing sale references product records " +
                        "that could not be loaded for this tenant.");
                }

                var effectiveStockByProductId =
                    products.ToDictionary(
                        x => x.Id,
                        x => x.CurrentStock);

                foreach (var oldItem in oldItems)
                {
                    effectiveStockByProductId[
                        oldItem.ProductId] +=
                        oldItem.Quantity;
                }

                foreach (var item in request.Items)
                {
                    var product =
                        productById[item.ProductId];

                    var availableStock =
                        effectiveStockByProductId[
                            item.ProductId];

                    if (availableStock < item.Quantity)
                    {
                        await transaction.RollbackAsync(
                            cancellationToken);

                        return ApiResponse<SaleDto>
                            .FailureResponse(
                                new List<string>
                                {
                                    $"Insufficient stock for " +
                                    $"'{product.Name}'. " +
                                    $"Available: {availableStock}, " +
                                    $"Requested: {item.Quantity}"
                                });
                    }

                    effectiveStockByProductId[
                        item.ProductId] -=
                        item.Quantity;
                }

                foreach (var oldItem in oldItems)
                {
                    var oldProduct =
                        productById[oldItem.ProductId];

                    oldProduct.CurrentStock +=
                        oldItem.Quantity;

                    await _stockMovementService.CreateMovement(
                        oldProduct.Id,
                        oldItem.Quantity,
                        StockMovementType.SaleRestore.ToString(),
                        oldProduct.CurrentStock,
                        "Sale updated - old sale reversed",
                        sale.InvoiceNumber);
                }

                _context.SaleItems.RemoveRange(oldItems);

                await _context.SaveChangesAsync(
                    cancellationToken);

                sale.SaleItems.Clear();

                decimal subTotal = 0;
                decimal totalTax = 0;
                decimal totalCost = 0;

                sale.CustomerId = request.CustomerId;
                sale.SaleDate = request.SaleDate;
                sale.DiscountAmount =
                    request.DiscountAmount;

                foreach (var item in request.Items)
                {
                    var product =
                        productById[item.ProductId];

                    product.CurrentStock -=
                        item.Quantity;

                    await _stockMovementService.CreateMovement(
                        product.Id,
                        item.Quantity,
                        StockMovementType.Sale.ToString(),
                        product.CurrentStock,
                        "Sale updated - new sale applied",
                        sale.InvoiceNumber,
                        cancellationToken);

                    var itemSubTotal =
                        product.SellingPrice
                        * item.Quantity;

                    var taxAmount =
                        (itemSubTotal
                        * product.TaxRate)
                        / 100;

                    var totalPrice =
                        itemSubTotal
                        + taxAmount;

                    var itemCost =
                        product.CostPrice
                        * item.Quantity;

                    var profit =
                        itemSubTotal
                        - itemCost;

                    var saleItem =
                        new SaleItem
                        {
                            TenantId = tenantId,
                            SaleId = sale.Id,
                            ProductId = product.Id,
                            Quantity = item.Quantity,
                            UnitPrice =
                                product.SellingPrice,
                            CostPrice =
                                product.CostPrice,
                            TaxRate =
                                product.TaxRate,
                            TaxAmount =
                                taxAmount,
                            TotalPrice =
                                totalPrice,
                            ProfitAmount =
                                profit
                        };

                    await _context.SaleItems
                        .AddAsync(
                            saleItem,
                            cancellationToken);

                    subTotal += itemSubTotal;
                    totalTax += taxAmount;
                    totalCost += itemCost;
                }

                var totalAmount =
                    subTotal
                    + totalTax
                    - request.DiscountAmount;

                if (totalAmount < 0)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return ApiResponse<SaleDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Discount amount cannot exceed " +
                                "the sale total"
                            });
                }

                sale.SubTotal = subTotal;
                sale.TaxAmount = totalTax;
                sale.TotalAmount = totalAmount;

                sale.ProfitAmount =
                    (sale.TotalAmount
                    - sale.TaxAmount)
                    - totalCost;

                await _context.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);

                return ApiResponse<SaleDto>
                    .SuccessResponse(
                        sale.ToSaleDto(),
                        "Sale updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(
                    CancellationToken.None);

                _context.ClearChangeTracker();

                return ApiResponse<SaleDto>
                    .FailureResponse(
                        new List<string>
                        {
                "Stock changed while the sale was being updated. " +
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