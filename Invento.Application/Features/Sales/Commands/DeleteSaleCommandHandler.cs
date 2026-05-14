using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Sales.Command;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Sales.Commands;

public class DeleteSaleCommandHandler
    : ICommandHandler<
        DeleteSaleCommand,
        ApiResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public DeleteSaleCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Guid>> Handle(
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
                    && !x.IsDeleted,
                    cancellationToken);

            if (sale is null)
            {
                return ApiResponse<Guid>
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
                        x.Id == item.ProductId,
                        cancellationToken);

                product.CurrentStock += item.Quantity;
            }

            sale.IsDeleted = true;

            await _context.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return ApiResponse<Guid>
                .SuccessResponse(
                    sale.Id,
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