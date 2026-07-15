using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Common.Extensions;
using Invento.Application.Features.Customer.Commands;
using Invento.Application.Features.Customers.DTOs;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Customers.Commands
{
    public class UpdateCustomerCommandHandler
        : ICommandHandler<UpdateCustomerCommand, ApiResponse<CustomerDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly ICacheVersionService _cacheVersionService;

        public UpdateCustomerCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            ICacheVersionService cacheVersionService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _cacheVersionService = cacheVersionService;
        }

        public async Task<ApiResponse<CustomerDto>> Handle(
                UpdateCustomerCommand request,
                CancellationToken cancellationToken)
        {
            var tenantId = _currentTenant.TenantId;

            var customer = await _context.Customers
                .FirstOrDefaultAsync(x =>
                    x.Id == request.Id
                    && x.TenantId
                        == _currentTenant.TenantId
                    && !x.IsDeleted,
                    cancellationToken
                );

            if (customer is null)
            {
                return ApiResponse<CustomerDto>
                    .FailureResponse(
                        new List<string>
                        {
                        "Customer not found"
                        }
                    );
            }

            customer.Name = request.Name;
            customer.Email = request.Email;
            customer.PhoneNumber = request.PhoneNumber;
            customer.Address = request.Address;

            await _context.SaveChangesAsync(cancellationToken);

            await _cacheVersionService.InvalidateAsync(
                    tenantId,
                    CacheGroups.Customers,
                    CacheGroups.Receivables,
                    CacheGroups.Reports,
                    CacheGroups.Dashboard);

            return ApiResponse<CustomerDto>
                .SuccessResponse(
                    new CustomerDto
                    {
                        Id = customer.Id,
                        Name = customer.Name,
                        Email = customer.Email,
                        PhoneNumber = customer.PhoneNumber,
                        Address = customer.Address,
                        IsDeleted = customer.IsDeleted
                    },
                    "Customer updated successfully"
                );
        }
    }
}