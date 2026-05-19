using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Sales.Command;
using Invento.Application.Features.Sales.DTOs;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Sales.Commands;

public class DeleteSaleCommandHandler
    : ICommandHandler<
        DeleteSaleCommand,
        ApiResponse<SaleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentTenantService _currentTenant;

    public DeleteSaleCommandHandler(
        IApplicationDbContext context,
        ICurrentTenantService currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<ApiResponse<SaleDto>> Handle(
        DeleteSaleCommand request,
        CancellationToken cancellationToken)
    {
        using var transaction =
            await _context.BeginTransactionAsync();

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
                        });
            }

            foreach (var item in sale.SaleItems)
            {
                var product = await _context.Products
                    .FirstAsync(x =>
                        x.Id == item.ProductId
                        && x.TenantId == _currentTenant.TenantId,
                        cancellationToken);

                product.CurrentStock += item.Quantity;
            }

            sale.IsDeleted = true;

            await _context.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return ApiResponse<SaleDto>
                .SuccessResponse(
                new SaleDto
                {

                },
                "Sale deleted successfully");
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }
}