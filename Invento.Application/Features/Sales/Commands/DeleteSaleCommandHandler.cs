using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Common.Extensions;
using Invento.Application.Common.Services;
using Invento.Application.Features.Sales.Command;
using Invento.Application.Features.Sales.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Sales.Commands
{
    public class DeleteSaleCommandHandler
        : ICommandHandler<
            DeleteSaleCommand,
            ApiResponse<DeleteSaleDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly StockMovementService _stockMovementService;
        private readonly ICacheVersionService _cacheVersionService;

        public DeleteSaleCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            StockMovementService stockMovementService,
            ICacheVersionService cacheVersionService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _stockMovementService = stockMovementService;
            _cacheVersionService = cacheVersionService;
        }

        public async Task<ApiResponse<DeleteSaleDto>> Handle(
            DeleteSaleCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId =
                _currentTenant.TenantId;

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

                            _context.ClearChangeTracker();

                            return ApiResponse<DeleteSaleDto>
                                .FailureResponse(
                                    new List<string>
                                    {
                                "Sale not found"
                                    });
                        }

                        if (sale.PaidAmount > 0)
                        {
                            await transaction.RollbackAsync(
                                cancellationToken);

                            _context.ClearChangeTracker();

                            return ApiResponse<DeleteSaleDto>
                                .FailureResponse(
                                    new List<string>
                                    {
                                "Cannot delete a sale that has " +
                                "received payment. Payment reversal " +
                                "must be completed first."
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
                                        && x.TenantId == tenantId)
                                .ToListAsync(
                                    cancellationToken);

                        var productById =
                            products.ToDictionary(
                                x => x.Id);

                        var missingProductIds =
                            productIds
                                .Where(
                                    id =>
                                        !productById.ContainsKey(id))
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
                                                $"Product not found: {id}")
                                        .ToList());
                        }

                        foreach (var item in saleItems)
                        {
                            var product =
                                productById[item.ProductId];

                            product.CurrentStock +=
                                item.Quantity;

                            await _stockMovementService
                                .CreateMovement(
                                    product.Id,
                                    item.Quantity,
                                    StockMovementType
                                        .SaleRestore
                                        .ToString(),
                                    product.CurrentStock,
                                    "Sale deleted",
                                    sale.InvoiceNumber,
                                    cancellationToken);
                        }

                        sale.IsDeleted = true;

                        await _context.SaveChangesAsync(
                            cancellationToken);

                        await _cacheVersionService.InvalidateAsync(
                                tenantId,
                                CacheGroups.Sales,
                                CacheGroups.Receivables,
                                CacheGroups.Balance,
                                CacheGroups.Products,
                                CacheGroups.Reports,
                                CacheGroups.Dashboard);

                        await transaction.CommitAsync(
                            cancellationToken);

                        return ApiResponse<DeleteSaleDto>
                            .SuccessResponse(
                                new DeleteSaleDto
                                {
                                    Id = sale.Id,
                                    InvoiceNumber =
                                        sale.InvoiceNumber,
                                    IsDeleted = true
                                },
                                "Sale deleted successfully");
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
                            "being deleted. Reload the latest " +
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