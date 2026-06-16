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

                var supplier = await _context.Suppliers
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == request.SupplierId
                            && x.TenantId == _currentTenant.TenantId
                            && !x.IsDeleted,
                        cancellationToken);

                if (supplier is null)
                {
                    return ApiResponse<PurchaseDto>
                        .FailureResponse(
                            ["Supplier not found"]);
                }

                foreach (var oldItem in purchase.PurchaseItems)
                {
                    var product = await _context.Products
                        .FirstAsync(
                            x =>
                                x.Id == oldItem.ProductId
                                && x.TenantId == _currentTenant.TenantId,
                            cancellationToken);

                    if (product.CurrentStock < oldItem.Quantity)
                    {
                        return ApiResponse<PurchaseDto>
                            .FailureResponse(
                                [$"Cannot update purchase. Product '{product.Name}' stock already consumed."]);
                    }

                    product.CurrentStock -= oldItem.Quantity;

                    await _stockMovementService.CreateMovement(
                        product.Id,
                        oldItem.Quantity,
                        StockMovementType.PurchaseReturn.ToString(),
                        product.CurrentStock,
                        "Purchase update reversal",
                        purchase.PurchaseNumber);
                }

                _context.PurchaseItems.RemoveRange(
                    purchase.PurchaseItems);

                await _context.SaveChangesAsync(cancellationToken);

                decimal subTotal = 0;
                decimal totalTax = 0;

                purchase.SupplierId = request.SupplierId;
                purchase.PurchaseDate = request.PurchaseDate;
                purchase.DiscountAmount = request.DiscountAmount;

                foreach (var item in request.Items)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id == item.ProductId
                                && x.TenantId == _currentTenant.TenantId
                                && !x.IsDeleted,
                            cancellationToken);

                    if (product is null)
                    {
                        return ApiResponse<PurchaseDto>
                            .FailureResponse(
                                [$"Product not found : {item.ProductId}"]);
                    }

                    product.CurrentStock += item.Quantity;
                    product.CostPrice = item.UnitCost;

                    var itemSubTotal =
                        item.UnitCost * item.Quantity;

                    var taxAmount =
                        (itemSubTotal * item.TaxRate) / 100;

                    var totalPrice =
                        itemSubTotal + taxAmount;

                    await _context.PurchaseItems.AddAsync(
                        new PurchaseItem
                        {
                            TenantId = _currentTenant.TenantId,
                            PurchaseId = purchase.Id,
                            ProductId = product.Id,
                            Quantity = item.Quantity,
                            UnitCost = item.UnitCost,
                            TaxRate = item.TaxRate,
                            TaxAmount = taxAmount,
                            TotalPrice = totalPrice
                        },
                        cancellationToken);

                    await _stockMovementService.CreateMovement(
                        product.Id,
                        item.Quantity,
                        StockMovementType.Purchase.ToString(),
                        product.CurrentStock,
                        "Purchase updated",
                        purchase.PurchaseNumber);

                    subTotal += itemSubTotal;
                    totalTax += taxAmount;
                }

                purchase.SubTotal = subTotal;
                purchase.TaxAmount = totalTax;
                purchase.TotalAmount =
                    subTotal + totalTax - request.DiscountAmount;

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return ApiResponse<PurchaseDto>
                    .SuccessResponse(
                        purchase.ToPurchaseDto(),
                        "Purchase updated successfully");
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}