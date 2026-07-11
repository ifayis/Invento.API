using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Services;
using Invento.Application.Features.Sales.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Sales.Commands
{
    public class RestoreSaleCommandHandler
        : ICommandHandler<
            RestoreSaleCommand,
            ApiResponse<DeleteSaleDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly StockMovementService _stockMovementService;

        public RestoreSaleCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            StockMovementService stockMovementService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _stockMovementService = stockMovementService;
        }

        public async Task<ApiResponse<DeleteSaleDto>> Handle(
            RestoreSaleCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentTenant.TenantId;

            var strategy =
                _context.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(
                async () =>
                {

                    await using var transaction =
                await _context.BeginTransactionAsync(
                    cancellationToken);

                    try
                    {
                        var sale =
                            await _context.Sales
                                .IgnoreQueryFilters()
                                .Include(x => x.SaleItems)
                                .FirstOrDefaultAsync(
                                    x =>
                                        x.Id == request.Id
                                        && x.TenantId == tenantId
                                        && x.IsDeleted,
                                    cancellationToken);

                        if (sale is null)
                        {
                            await transaction.RollbackAsync(
                                cancellationToken);

                            _context.ClearChangeTracker();

                            return ApiResponse<DeleteSaleDto>
                                .FailureResponse(
                                    new List<string>
                                    {
                                "Hidden sale not found"
                                    });
                        }

                        var saleItems =
                            sale.SaleItems.ToList();

                        var productIds =
                            saleItems
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

                            return ApiResponse<DeleteSaleDto>
                                .FailureResponse(
                                    missingProductIds
                                        .Select(
                                            id =>
                                                $"Product not found or inactive: {id}")
                                        .ToList());
                        }

                        var requiredQuantityByProductId =
                            saleItems
                                .GroupBy(x => x.ProductId)
                                .ToDictionary(
                                    group => group.Key,
                                    group => group.Sum(
                                        item => item.Quantity));

                        foreach (var requiredStock in
                            requiredQuantityByProductId)
                        {
                            var product =
                                productById[
                                    requiredStock.Key];

                            if (product.CurrentStock <
                                requiredStock.Value)
                            {
                                await transaction.RollbackAsync(
                                    cancellationToken);

                                _context.ClearChangeTracker();

                                return ApiResponse<DeleteSaleDto>
                                    .FailureResponse(
                                        new List<string>
                                        {
                                    $"Insufficient stock to restore " +
                                    $"sale for product " +
                                    $"'{product.Name}'. " +
                                    $"Available: " +
                                    $"{product.CurrentStock}, " +
                                    $"required: " +
                                    $"{requiredStock.Value}."
                                        });
                            }
                        }

                        foreach (var item in saleItems)
                        {
                            var product =
                                productById[item.ProductId];

                            product.CurrentStock -=
                                item.Quantity;

                            await _stockMovementService
                                .CreateMovement(
                                    product.Id,
                                    item.Quantity,
                                    StockMovementType
                                        .Sale
                                        .ToString(),
                                    product.CurrentStock,
                                    "Sale restored",
                                    sale.InvoiceNumber,
                                    cancellationToken);
                        }

                        sale.IsDeleted = false;

                        await _context.SaveChangesAsync(
                            cancellationToken);

                        await transaction.CommitAsync(
                            cancellationToken);

                        return ApiResponse<DeleteSaleDto>
                            .SuccessResponse(
                                new DeleteSaleDto
                                {
                                    Id = sale.Id,
                                    InvoiceNumber =
                                        sale.InvoiceNumber,
                                    IsDeleted = false
                                },
                                "Sale restored successfully");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        await transaction.RollbackAsync(
                            CancellationToken.None);

                        _context.ClearChangeTracker();

                        return ApiResponse<DeleteSaleDto>
                            .FailureResponse(
                                new List<string>
                                {
                            "Stock changed while the sale was " +
                            "being restored. Reload the latest " +
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
            );
        }
    }
}