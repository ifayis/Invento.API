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
    public class CreateSaleCommandHandler
        : ICommandHandler<
            CreateSaleCommand,
            ApiResponse<SaleDetailsDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly StockMovementService _stockMovementService;
        private readonly CashTransactionService _cashTransactionService;

        public CreateSaleCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            StockMovementService stockMovementService,
            CashTransactionService cashTransactionService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _stockMovementService = stockMovementService;
            _cashTransactionService = cashTransactionService;
        }

        public async Task<ApiResponse<SaleDetailsDto>> Handle(
            CreateSaleCommand request,
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

                    _context.ClearChangeTracker();

                    return ApiResponse<SaleDetailsDto>
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

                    _context.ClearChangeTracker();

                    return ApiResponse<SaleDetailsDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Discount amount cannot be negative"
                            });
                }

                if (request.PaidAmount < 0)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    _context.ClearChangeTracker();

                    return ApiResponse<SaleDetailsDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Paid amount cannot be negative"
                            });
                }

                if (request.Items.Any(
                    x => x.Quantity <= 0))
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    _context.ClearChangeTracker();

                    return ApiResponse<SaleDetailsDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "All item quantities must be " +
                                "greater than zero"
                            });
                }

                var duplicateProductIds =
                    request.Items
                        .GroupBy(x => x.ProductId)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key)
                        .ToList();

                if (duplicateProductIds.Count > 0)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    _context.ClearChangeTracker();

                    return ApiResponse<SaleDetailsDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Duplicate products are not allowed " +
                                "in the same sale request"
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

                        _context.ClearChangeTracker();

                        return ApiResponse<SaleDetailsDto>
                            .FailureResponse(
                                new List<string>
                                {
                                    "Customer not found"
                                });
                    }
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

                    _context.ClearChangeTracker();

                    return ApiResponse<SaleDetailsDto>
                        .FailureResponse(
                            missingProductIds
                                .Select(
                                    id =>
                                        $"Product not found: {id}")
                                .ToList());
                }

                foreach (var item in request.Items)
                {
                    var product =
                        productById[item.ProductId];

                    if (product.CurrentStock <
                        item.Quantity)
                    {
                        await transaction.RollbackAsync(
                            cancellationToken);

                        _context.ClearChangeTracker();

                        return ApiResponse<SaleDetailsDto>
                            .FailureResponse(
                                new List<string>
                                {
                                    $"Insufficient stock for " +
                                    $"product '{product.Name}'. " +
                                    $"Available: " +
                                    $"{product.CurrentStock}, " +
                                    $"requested: {item.Quantity}."
                                });
                    }
                }

                var invoiceNumber =
                    $"INV-{DateTime.UtcNow.Ticks}";

                decimal subTotal = 0;
                decimal totalTax = 0;
                decimal totalCost = 0;

                var sale =
                    new Sale
                    {
                        TenantId = tenantId,
                        CustomerId = request.CustomerId,
                        InvoiceNumber = invoiceNumber,
                        SaleDate = request.SaleDate,
                        DiscountAmount =
                            request.DiscountAmount
                    };

                foreach (var item in request.Items)
                {
                    var product =
                        productById[item.ProductId];

                    product.CurrentStock -=
                        item.Quantity;

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

                    var profitAmount =
                        itemSubTotal
                        - itemCost;

                    var saleItem =
                        new SaleItem
                        {
                            TenantId = tenantId,
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
                                profitAmount
                        };

                    sale.SaleItems.Add(
                        saleItem);

                    await _stockMovementService
                        .CreateMovement(
                            product.Id,
                            item.Quantity,
                            StockMovementType
                                .Sale
                                .ToString(),
                            product.CurrentStock,
                            "Sale completed",
                            sale.InvoiceNumber,
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

                    _context.ClearChangeTracker();

                    return ApiResponse<SaleDetailsDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Discount amount cannot exceed " +
                                "the sale total"
                            });
                }

                if (request.PaidAmount > totalAmount)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    _context.ClearChangeTracker();

                    return ApiResponse<SaleDetailsDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Paid amount cannot exceed " +
                                "total amount"
                            });
                }

                sale.SubTotal = subTotal;
                sale.TaxAmount = totalTax;
                sale.TotalAmount = totalAmount;
                sale.PaidAmount = request.PaidAmount;
                sale.DueAmount =
                    totalAmount
                    - request.PaidAmount;

                sale.PaymentStatus =
                    sale.DueAmount == 0
                        ? PaymentStatus.Paid
                        : request.PaidAmount == 0
                            ? PaymentStatus.Unpaid
                            : PaymentStatus.PartiallyPaid;

                sale.ProfitAmount =
                    (sale.TotalAmount - sale.TaxAmount)
                    - totalCost;

                await _context.Sales.AddAsync(
                    sale,
                    cancellationToken);

                if (request.PaidAmount > 0)
                {
                    await _cashTransactionService
                        .CreateTransaction(
                            CashTransactionType.Sale,
                            request.PaidAmount,
                            $"Sale Payment - " +
                            $"{sale.InvoiceNumber}",
                            request.SaleDate);
                }

                if (request.CustomerId.HasValue
                    && request.PaidAmount > 0)
                {
                    sale.Payments.Add(
                        new CustomerPayment
                        {
                            TenantId = tenantId,
                            CustomerId =
                                request.CustomerId.Value,
                            Amount =
                                request.PaidAmount,
                            PaymentDate =
                                request.SaleDate,
                            Remarks =
                                "Initial payment"
                        });
                }

                await _context.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);

                return ApiResponse<SaleDetailsDto>
                    .SuccessResponse(
                        sale.ToSaleDetailsDto(),
                        "Sale created successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(
                    CancellationToken.None);

                _context.ClearChangeTracker();

                return ApiResponse<SaleDetailsDto>
                    .FailureResponse(
                        new List<string>
                        {
                            "Stock changed while the sale was " +
                            "being processed. Reload the latest " +
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