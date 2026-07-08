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
    public class CreatePurchaseCommandHandler
        : ICommandHandler<
            CreatePurchaseCommand,
            ApiResponse<PurchaseDetailsDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly StockMovementService _stockMovementService;

        public CreatePurchaseCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            StockMovementService stockMovementService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _stockMovementService = stockMovementService;
        }

        public async Task<ApiResponse<PurchaseDetailsDto>> Handle(
            CreatePurchaseCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentTenant.TenantId;

            await using var transaction =
                await _context.BeginTransactionAsync(
                    cancellationToken);
            try
            {
                if (request.Items is null
                    || request.Items.Count == 0)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return ApiResponse<PurchaseDetailsDto>
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

                    return ApiResponse<PurchaseDetailsDto>
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

                    return ApiResponse<PurchaseDetailsDto>
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

                    return ApiResponse<PurchaseDetailsDto>
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

                    return ApiResponse<PurchaseDetailsDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Tax rate cannot be negative"
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

                    return ApiResponse<PurchaseDetailsDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Duplicate products are not allowed " +
                                "in the same purchase request"
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

                    return ApiResponse<PurchaseDetailsDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Supplier not found"
                            });
                }

                var productIds =
                    request.Items
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

                    return ApiResponse<PurchaseDetailsDto>
                        .FailureResponse(
                            missingProductIds
                                .Select(
                                    id =>
                                        $"Product not found: {id}")
                                .ToList());
                }

                var now = DateTime.UtcNow;

                var currentMonth =
                    now.ToString("yyyyMM");

                var purchaseCount =
                    await _context.Purchases
                        .IgnoreQueryFilters()
                        .CountAsync(
                            x =>
                                x.TenantId == tenantId
                                && x.PurchaseDate.Year == now.Year
                                && x.PurchaseDate.Month == now.Month,
                            cancellationToken);

                var purchaseNumber =
                    $"PUR-{currentMonth}-" +
                    $"{purchaseCount + 1:D4}";

                decimal subTotal = 0;
                decimal totalTax = 0;

                var purchase =
                    new Purchase
                    {
                        TenantId = tenantId,
                        SupplierId = request.SupplierId,
                        PurchaseDate = request.PurchaseDate,
                        PurchaseNumber = purchaseNumber,
                        DiscountAmount =
                            request.DiscountAmount
                    };

                foreach (var item in request.Items)
                {
                    var product =
                        productById[item.ProductId];

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
                            ProductId = product.Id,
                            Quantity = item.Quantity,
                            UnitCost = item.UnitCost,
                            TaxRate = item.TaxRate,
                            TaxAmount = taxAmount,
                            TotalPrice = totalPrice
                        };

                    purchase.PurchaseItems.Add(
                        purchaseItem);

                    await _stockMovementService
                        .CreateMovement(
                            product.Id,
                            item.Quantity,
                            StockMovementType.Purchase.ToString(),
                            product.CurrentStock,
                            "Purchase completed",
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

                    return ApiResponse<PurchaseDetailsDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Discount amount cannot exceed " +
                                "the purchase total"
                            });
                }

                purchase.SubTotal = subTotal;
                purchase.TaxAmount = totalTax;
                purchase.TotalAmount = totalAmount;
                purchase.PaidAmount = 0;
                purchase.DueAmount = totalAmount;
                purchase.PaymentStatus =
                    PaymentStatus.Unpaid;

                await _context.Purchases.AddAsync(
                    purchase,
                    cancellationToken);

                await _context.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);

                return ApiResponse<PurchaseDetailsDto>
                    .SuccessResponse(
                        purchase.ToPurchaseDetailsDto(),
                        "Purchase created successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(
                    CancellationToken.None);

                _context.ClearChangeTracker();

                return ApiResponse<PurchaseDetailsDto>
                    .FailureResponse(
                        new List<string>
                        {
                "Stock changed while the purchase was being processed. " +
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