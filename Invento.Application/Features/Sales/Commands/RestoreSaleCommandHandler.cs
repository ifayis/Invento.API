using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Sales.DTOs;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Sales.Commands
{
    public class RestoreSaleCommandHandler
        : ICommandHandler<RestoreSaleCommand, ApiResponse<DeleteSaleDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;

        public RestoreSaleCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant)
        {
            _context = context;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<DeleteSaleDto>> Handle(
            RestoreSaleCommand request,
            CancellationToken cancellationToken)
        {
            var sale =
                await _context.Sales
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id
                    && x.TenantId == _currentTenant.TenantId
                    && x.IsDeleted,
                    cancellationToken
                );

            if (sale is null)
            {
                return ApiResponse<DeleteSaleDto>
                    .FailureResponse(
                        new()
                        {
                            "Hidden sale not found"
                        }
                    );
            }

            sale.IsDeleted = false;

            await _context.SaveChangesAsync(
                cancellationToken);

            return ApiResponse<DeleteSaleDto>
                .SuccessResponse(
                    new DeleteSaleDto
                    {
                        Id = sale.Id,
                        InvoiceNumber = sale.InvoiceNumber,
                        IsDeleted = false
                    },
                    "Sale restored successfully"
                );
        }
    }
}