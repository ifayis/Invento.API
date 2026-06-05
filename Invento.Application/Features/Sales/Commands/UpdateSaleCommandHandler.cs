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
        : ICommandHandler<UpdateSaleCommand, ApiResponse<SaleDto>>
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
            using var transaction =
                await _context.BeginTransactionAsync();

            try
            {
                var sale = await _context.Sales
                    .Include(x => x.SaleItems)
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == request.Id
                            && x.TenantId ==
                                _currentTenant.TenantId
                            && !x.IsDeleted,
                        cancellationToken
                    );

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

                foreach (var oldItem in sale.SaleItems)
                {
                    var oldProduct =
                        await _context.Products
                        .FirstAsync(
                            x =>
                                x.Id == oldItem.ProductId
                                && x.TenantId ==
                                    _currentTenant.TenantId,
                            cancellationToken
                        );

                    oldProduct.CurrentStock += oldItem.Quantity;

                    await _stockMovementService.CreateMovement(
                        oldProduct.Id,
                        oldItem.Quantity,
                        StockMovementType.SaleRestore.ToString(),
                        oldProduct.CurrentStock,
                        "Sale updated - old sale reversed",
                        sale.InvoiceNumber
                    );
                }

                _context.SaleItems.RemoveRange(sale.SaleItems);

                await _context.SaveChangesAsync(cancellationToken);

                foreach (var item in sale.SaleItems.ToList())
                {
                    _context.SaleItems.Local.Remove(item);
                }

                sale.SaleItems.Clear();

                decimal subTotal = 0;
                decimal totalTax = 0;
                decimal totalCost = 0;

                if (request.CustomerId.HasValue)
                {
                    var customer = await _context.Customers
                        .FirstOrDefaultAsync(
                            x => x.Id == request.CustomerId
                            && x.TenantId == _currentTenant.TenantId
                            && !x.IsDeleted,
                            cancellationToken);

                    if (customer is null)
                    {
                        return ApiResponse<SaleDto>
                            .FailureResponse(
                                new List<string>
                                {
                    "Customer not found"
                                });
                    }
                }

                sale.CustomerId = request.CustomerId;
                sale.SaleDate = request.SaleDate;
                sale.DiscountAmount = request.DiscountAmount;

                foreach (var item in request.Items)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync( x =>
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

                    if (item.Quantity <= 0)
                    {
                        return ApiResponse<SaleDto>
                            .FailureResponse(
                                new List<string>
                                {
                                    $"Quantity must be greater than zero for {product.Name}"
                                }
                            );
                    }

                    if (product.CurrentStock < item.Quantity)
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

                    await _stockMovementService.CreateMovement(
                        product.Id,
                        item.Quantity,
                        StockMovementType.Sale.ToString(),
                        product.CurrentStock,
                        "Sale updated - new sale applied",
                        sale.InvoiceNumber
                    );

                    var itemSubTotal = product.SellingPrice * item.Quantity;

                    var taxAmount = (itemSubTotal * product.TaxRate) / 100;

                    var totalPrice = itemSubTotal + taxAmount;

                    var itemCost = product.CostPrice * item.Quantity;

                    var profit = itemSubTotal - itemCost;

                    var saleItem =
                        new SaleItem
                        {
                            TenantId = _currentTenant.TenantId,
                            SaleId = sale.Id,
                            ProductId = product.Id,
                            Quantity = item.Quantity,
                            UnitPrice = product.SellingPrice,
                            CostPrice = product.CostPrice,
                            TaxRate = product.TaxRate,
                            TaxAmount = taxAmount,
                            TotalPrice = totalPrice,
                            ProfitAmount = profit
                        };

                    await _context.SaleItems
                        .AddAsync(
                            saleItem,
                            cancellationToken
                        );

                    subTotal += itemSubTotal;
                    totalTax += taxAmount;
                    totalCost += itemCost;
                }

                sale.SubTotal = subTotal;
                sale.TaxAmount = totalTax;
                sale.TotalAmount = subTotal + totalTax - request.DiscountAmount;
                sale.ProfitAmount = (sale.TotalAmount - sale.TaxAmount) - totalCost;

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return ApiResponse<SaleDto>
                    .SuccessResponse(
                        sale.ToSaleDto(),
                        "Sale updated successfully"
                    );
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);

                throw;
            }
        }
    }
}