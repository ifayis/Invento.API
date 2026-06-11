using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Suppliers.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Suppliers.Commands
{
    public class CreateSupplierCommandHandler
        : ICommandHandler<
            CreateSupplierCommand,
            ApiResponse<SupplierDto>>
    {
        private readonly IApplicationDbContext _context;

        private readonly ICurrentTenantService _currentTenant;

        public CreateSupplierCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant)
        {
            _context = context;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<SupplierDto>> Handle(
            CreateSupplierCommand request,
            CancellationToken cancellationToken)
        {
            var exists = await _context.Suppliers
                .AnyAsync(
                    x =>
                        x.TenantId ==
                            _currentTenant.TenantId
                        &&
                        x.Name == request.Name,
                    cancellationToken
                );

            if (exists)
            {
                return ApiResponse<SupplierDto>
                    .FailureResponse(
                        new()
                        {
                            "Supplier already exists"
                        });
            }

            var supplier = new Supplier
            {
                TenantId = _currentTenant.TenantId,

                Name = request.Name.Trim(),

                ContactPerson =
                    request.ContactPerson?.Trim(),

                Email =
                    request.Email?.Trim(),

                PhoneNumber =
                    request.PhoneNumber?.Trim(),

                Address =
                    request.Address?.Trim(),

                TaxRegistrationNumber =
                    request.TaxRegistrationNumber?.Trim()
            };

            await _context.Suppliers.AddAsync(
                supplier,
                cancellationToken);

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
                        CreatedAt =
                            supplier.CreatedAt
                    },
                    "Supplier created successfully");
        }
    }
}