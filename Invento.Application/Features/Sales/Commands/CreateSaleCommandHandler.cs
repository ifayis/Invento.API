using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Sales.Command;
using Invento.Application.Features.Sales.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Sales.Commands;

public class CreateSaleCommandHandler
    : ICommandHandler<CreateSaleCommand, ApiResponse<SaleDetailsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentTenantService _currentTenant;

    public CreateSaleCommandHandler(
        IApplicationDbContext context, 
        ICurrentTenantService currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
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

            var sale = new Sale
            {
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

                var profitAmount = ((product.SellingPrice - product.CostPrice) * item.Quantity);

                var oldMovements = await _context.StockMovements
                    .Where(x =>
                        x.ReferenceNumber == sale.InvoiceNumber
                        && x.TenantId == _currentTenant.TenantId)
                    .ToListAsync(cancellationToken);

                 _context.StockMovements.RemoveRange(oldMovements);

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

                var stockMovement =
                    new StockMovement
                    {
                        TenantId = _currentTenant.TenantId,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        MovementType = "StockOut",
                        Remarks = "Sale completed",
                        ReferenceNumber =sale.InvoiceNumber
                    };

                await _context.StockMovements.AddAsync(
                    new StockMovement
                    {
                       TenantId = _currentTenant.TenantId,

                         ProductId = product.Id,

                         Quantity = item.Quantity,

                         MovementType = "StockOut",

                         Remarks = "Sale updated",

                         ReferenceNumber = sale.InvoiceNumber

                    }, cancellationToken
                );

                subTotal += itemSubTotal;
                totalTax += taxAmount;
                totalProfit += profitAmount;
            }

            sale.SubTotal = subTotal;
            sale.TaxAmount = totalTax;
            sale.ProfitAmount = totalProfit;

            sale.TotalAmount = subTotal + totalTax - request.DiscountAmount;

            await _context.Sales.AddAsync(
                sale,
                cancellationToken);

            await _context.SaveChangesAsync( cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return ApiResponse<SaleDetailsDto>
                .SuccessResponse(
                new SaleDetailsDto
                {
                    InvoiceNumber = sale.InvoiceNumber,
                    SaleDate = sale.SaleDate,
                    TotalAmount = sale.TotalAmount
                },
                    "Sale created successfully"
                );
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }
}