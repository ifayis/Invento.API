using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Sales.Command;
using Invento.Application.Features.Sales.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Sales.Commands;

public class UpdateSaleCommandHandler
    : ICommandHandler<UpdateSaleCommand, ApiResponse<SaleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentTenantService _currentTenant;

    public UpdateSaleCommandHandler(
        IApplicationDbContext context,
        ICurrentTenantService currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<ApiResponse<SaleDto>> Handle(
        UpdateSaleCommand request,
        CancellationToken cancellationToken)
    {
        using var transaction = await _context.BeginTransactionAsync();

        try
        {
            var sale = await _context.Sales
                .Include(x => x.SaleItems)
                .FirstOrDefaultAsync(x =>
                    x.Id == request.Id
                    && x.TenantId == _currentTenant.TenantId
                    && !x.IsDeleted,
                    cancellationToken);

            if (sale is null)
            {
                return ApiResponse<SaleDto>
                    .FailureResponse(
                        new List<string>
                        {
                            "Sale not found"
                        }
                    );
            }

            foreach (var item in sale.SaleItems)
            {
                var product = await _context.Products
                    .FirstAsync(x =>
                        x.Id == item.ProductId
                        && x.TenantId == _currentTenant.TenantId,
                        cancellationToken
                    );

                product.CurrentStock += item.Quantity;
            }

            _context.SaleItems.RemoveRange(
                sale.SaleItems);

            decimal subTotal = 0;
            decimal totalTax = 0;
            decimal totalProfit = 0;

            sale.SaleDate = request.SaleDate;
            sale.DiscountAmount =
                request.DiscountAmount;

            sale.SaleItems.Clear();

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
                    return ApiResponse<SaleDto>
                        .FailureResponse(
                            new List<string>
                            {
                                $"Product not found: {item.ProductId}"
                            }
                        );
                }

                if (product.CurrentStock
                    < item.Quantity)
                {
                    return ApiResponse<SaleDto>
                        .FailureResponse(
                            new List<string>
                            {
                                $"Insufficient stock for {product.Name}"
                            }
                        );
                }

                product.CurrentStock -= item.Quantity;

                var itemSubTotal = product.SellingPrice * item.Quantity;

                var taxAmount = (itemSubTotal * product.TaxRate) / 100;

                var totalPrice = itemSubTotal + taxAmount;

                var profit = (product.SellingPrice - product.CostPrice) * item.Quantity;

                sale.SaleItems.Add(
                    new SaleItem
                    {  
                        TenantId = _currentTenant.TenantId,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.SellingPrice,
                        CostPrice = product.CostPrice,
                        TaxRate = product.TaxRate,
                        TaxAmount = taxAmount,
                        TotalPrice = totalPrice,
                        ProfitAmount = profit
                    });

                subTotal += itemSubTotal;
                totalTax += taxAmount;
                totalProfit += profit;
            }

            sale.SubTotal = subTotal;
            sale.TaxAmount = totalTax;
            sale.ProfitAmount = totalProfit;

            sale.TotalAmount = subTotal + totalTax - request.DiscountAmount;

            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return ApiResponse<SaleDto>
                .SuccessResponse(
                new SaleDto
                {
                    InvoiceNumber = sale.InvoiceNumber,
                    SaleDate = sale.SaleDate,
                    TotalAmount = sale.TotalAmount
                },
                "Sale updated successfully");
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);

            throw;
        }
    }
}