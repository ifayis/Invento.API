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
            using var transaction =
                await _context.BeginTransactionAsync();

            try
            {
                var supplier =
                    await _context.Suppliers
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == request.SupplierId
                            && x.TenantId ==
                                _currentTenant.TenantId
                            && !x.IsDeleted,
                        cancellationToken);

                if (supplier is null)
                {
                    return ApiResponse<PurchaseDetailsDto>
                        .FailureResponse(
                            new()
                            {
                                "Supplier not found"
                            });
                }

                var currentMonth =
                    DateTime.UtcNow.ToString("yyyyMM");

                var purchaseCount =
                    await _context.Purchases
                    .IgnoreQueryFilters()
                    .CountAsync(
                        x =>
                            x.TenantId ==
                            _currentTenant.TenantId
                            &&
                            x.PurchaseDate.Year
                                == DateTime.UtcNow.Year
                            &&
                            x.PurchaseDate.Month
                                == DateTime.UtcNow.Month,
                        cancellationToken);

                var purchaseNumber =
                    $"PUR-{currentMonth}-{(purchaseCount + 1):D4}";

                decimal subTotal = 0;
                decimal totalTax = 0;

                var purchase = new Purchase
                {
                    TenantId = _currentTenant.TenantId,
                    SupplierId = request.SupplierId,
                    PurchaseDate = request.PurchaseDate,
                    PurchaseNumber = purchaseNumber,
                    DiscountAmount = request.DiscountAmount
                };

                foreach (var item in request.Items)
                {
                    var product =
                        await _context.Products
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id == item.ProductId
                                &&
                                x.TenantId ==
                                    _currentTenant.TenantId
                                &&
                                !x.IsDeleted,
                            cancellationToken);

                    if (product is null)
                    {
                        return ApiResponse<PurchaseDetailsDto>
                            .FailureResponse(
                                new()
                                {
                                    $"Product not found : {item.ProductId}"
                                });
                    }

                    product.CurrentStock += item.Quantity;

                    product.CostPrice = item.UnitCost;

                    var itemSubTotal =
                        item.UnitCost * item.Quantity;

                    var taxAmount =
                        (itemSubTotal * item.TaxRate) / 100;

                    var totalPrice =
                        itemSubTotal + taxAmount;

                    var purchaseItem =
                        new PurchaseItem
                        {
                            TenantId =
                                _currentTenant.TenantId,

                            ProductId =
                                product.Id,

                            Quantity =
                                item.Quantity,

                            UnitCost =
                                item.UnitCost,

                            TaxRate =
                                item.TaxRate,

                            TaxAmount =
                                taxAmount,

                            TotalPrice =
                                totalPrice
                        };

                    purchase.PurchaseItems
                        .Add(purchaseItem);

                    await _stockMovementService
                        .CreateMovement(
                            product.Id,
                            item.Quantity,
                            StockMovementType.Purchase.ToString(),
                            product.CurrentStock,
                            "Purchase completed",
                            purchase.PurchaseNumber
                        );

                    subTotal += itemSubTotal;

                    totalTax += taxAmount;
                }

                purchase.SubTotal = subTotal;

                purchase.TaxAmount = totalTax;

                purchase.TotalAmount =
                    subTotal
                    + totalTax
                    - request.DiscountAmount;

                purchase.PaidAmount = 0;

                purchase.DueAmount = purchase.TotalAmount;

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
            catch
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                throw;
            }
        }
    }
}