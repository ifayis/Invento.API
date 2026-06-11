using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Suppliers.DTOs;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Suppliers.Commands
{
    public class UpdateSupplierCommandHandler
        : ICommandHandler<
            UpdateSupplierCommand,
            ApiResponse<SupplierDto>>
    {
        private readonly IApplicationDbContext _context;

        private readonly ICurrentTenantService _currentTenant;

        public UpdateSupplierCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant)
        {
            _context = context;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<SupplierDto>> Handle(
            UpdateSupplierCommand request,
            CancellationToken cancellationToken)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == request.Id
                        &&
                        x.TenantId ==
                            _currentTenant.TenantId
                        &&
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

            supplier.Name = request.Name.Trim();

            supplier.ContactPerson =
                request.ContactPerson?.Trim();

            supplier.Email =
                request.Email?.Trim();

            supplier.PhoneNumber =
                request.PhoneNumber?.Trim();

            supplier.Address =
                request.Address?.Trim();

            supplier.TaxRegistrationNumber =
                request.TaxRegistrationNumber?.Trim();

            await _context.SaveChangesAsync(
                cancellationToken);

            return ApiResponse<SupplierDto>
                .SuccessResponse(
                    new SupplierDto
                    {
                        Id = supplier.Id,
                        Name = supplier.Name,
                        ContactPerson =
                            supplier.ContactPerson,
                        Email = supplier.Email,
                        PhoneNumber =
                            supplier.PhoneNumber,
                        Address =
                            supplier.Address,
                        TaxRegistrationNumber =
                            supplier.TaxRegistrationNumber,
                        IsDeleted =
                            supplier.IsDeleted,
                        CreatedAt =
                            supplier.CreatedAt
                    },
                    "Supplier updated successfully");
        }
    }
}