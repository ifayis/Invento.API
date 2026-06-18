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

namespace Invento.Application.Features.Sales.Commands;

public class CreateSaleCommandHandler
    : ICommandHandler<CreateSaleCommand, ApiResponse<SaleDetailsDto>>
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
        using var transaction = await _context.BeginTransactionAsync();

        try
        {
            decimal subTotal = 0;
            decimal totalTax = 0;
            decimal totalProfit = 0;
            decimal totalCost = 0;

            Invento.Domain.Entities.Customer? customer = null;

            if (request.CustomerId.HasValue)
            {
                customer = await _context.Customers
                    .FirstOrDefaultAsync(
                    x => x.Id == request.CustomerId
                    && x.TenantId == _currentTenant.TenantId
                    && !x.IsDeleted,
                    cancellationToken);

                if (customer is null)
                {
                    return ApiResponse<SaleDetailsDto>
                        .FailureResponse(
                        new List<string>
                        {
                "Customer not found"
                        });
                }
            }

            var sale = new Sale
            {
                TenantId = _currentTenant.TenantId,
                CustomerId = request.CustomerId,
                InvoiceNumber = $"INV-{DateTime.UtcNow.Ticks}",
                SaleDate = request.SaleDate,
                DiscountAmount = request.DiscountAmount
            };

            foreach (var item in request.Items)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(x =>
                            x.Id == item.ProductId
                            && x.TenantId == _currentTenant.TenantId
                            && !x.IsDeleted,
                        cancellationToken
                    );

                if (product is null)
                {
                    return ApiResponse<SaleDetailsDto>
                        .FailureResponse(
                            new List<string>
                            {
                                $"Product not found: {item.ProductId}"
                            }
                        );
                }

                if (product.CurrentStock < item.Quantity)
                {
                    return ApiResponse<SaleDetailsDto>
                        .FailureResponse(
                            new List<string>
                            {
                                $"Insufficient stock for product: {product.Name}"
                            }
                        );
                }

                product.CurrentStock -= item.Quantity;

                var itemSubTotal = product.SellingPrice * item.Quantity;

                var taxAmount = (itemSubTotal * product.TaxRate) / 100;

                var totalPrice = itemSubTotal + taxAmount;

                var itemCost = product.CostPrice * item.Quantity;

                var profitAmount = itemSubTotal - itemCost;

                var saleItem = new SaleItem
                {
                    TenantId = _currentTenant.TenantId,
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice =product.SellingPrice,
                    CostPrice =product.CostPrice,
                    TaxRate =product.TaxRate,
                    TaxAmount = taxAmount,
                    TotalPrice = totalPrice,
                    ProfitAmount = profitAmount
                };

                sale.SaleItems.Add(saleItem);

                await _stockMovementService.CreateMovement(
                    product.Id,
                    item.Quantity,
                    StockMovementType.Sale.ToString(),
                    product.CurrentStock,
                    "Sale completed",
                    sale.InvoiceNumber
                );
                subTotal += itemSubTotal;
                totalTax += taxAmount;
                totalCost += itemCost;
                totalProfit += profitAmount;
            }

            sale.SubTotal = subTotal;
            sale.TaxAmount = totalTax;
            sale.TotalAmount = subTotal + totalTax - request.DiscountAmount;

            if (request.PaidAmount > sale.TotalAmount)
            {
                return ApiResponse<SaleDetailsDto>
                    .FailureResponse(
                        new()
                        {
                "Paid amount cannot exceed total amount"
                        });
            }

            sale.PaidAmount = request.PaidAmount;
            sale.DueAmount = sale.TotalAmount - request.PaidAmount;

            sale.PaymentStatus = sale.DueAmount == 0
                    ? PaymentStatus.Paid
                    : request.PaidAmount == 0
                        ? PaymentStatus.Unpaid
                        : PaymentStatus.PartiallyPaid;

            if (request.PaidAmount > 0)
            {
                await _cashTransactionService.CreateTransaction(
                    CashTransactionType.Sale,
                    request.PaidAmount,
                    $"Sale Payment - {sale.InvoiceNumber}",
                    request.SaleDate);
            }

            sale.ProfitAmount = (sale.TotalAmount - sale.TaxAmount) - totalCost; 

            await _context.Sales.AddAsync(
                sale,
                cancellationToken
            );

            if (request.CustomerId.HasValue
                && request.PaidAmount > 0)
            {
                sale.Payments.Add(
                    new CustomerPayment
                    {
                        TenantId = _currentTenant.TenantId,
                        CustomerId = request.CustomerId.Value,
                        Amount = request.PaidAmount,
                        PaymentDate = request.SaleDate,
                        Remarks = "Initial payment"
                    });
            }

            await _context.SaveChangesAsync( cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return ApiResponse<SaleDetailsDto>
            .SuccessResponse(
                sale.ToSaleDetailsDto(),
                "Sale created successfully"
            );
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);

            throw;
        }
    }
}