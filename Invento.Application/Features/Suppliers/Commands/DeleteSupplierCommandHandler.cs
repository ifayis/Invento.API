using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Suppliers.DTOs;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Suppliers.Commands
{
    public class DeleteSupplierCommandHandler
        : ICommandHandler<
            DeleteSupplierCommand,
            ApiResponse<SupplierDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;

        public DeleteSupplierCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant)
        {
            _context = context;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<SupplierDto>> Handle(
            DeleteSupplierCommand request,
            CancellationToken cancellationToken)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == request.Id &&
                        x.TenantId == _currentTenant.TenantId &&
                        !x.IsDeleted,
                    cancellationToken);

            if (supplier is null)
            {
                return ApiResponse<SupplierDto>
                    .FailureResponse(
                        new()
                        {
                            "Supplier not found"
                        });
            }

            supplier.IsDeleted = true;

            await _context.SaveChangesAsync(
                cancellationToken);

            return ApiResponse<SupplierDto>
                .SuccessResponse(
                    new SupplierDto
                    {
                        Id = supplier.Id,
                        Name = supplier.Name,
                        ContactPerson = supplier.ContactPerson,
                        Email = supplier.Email,
                        PhoneNumber = supplier.PhoneNumber,
                        Address = supplier.Address,
                        TaxRegistrationNumber =
                            supplier.TaxRegistrationNumber,
                        IsDeleted = true,
                        CreatedAt = supplier.CreatedAt
                    },
                    "Supplier deleted successfully");
        }
    }
}