using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Common.Extensions;
using Invento.Application.Features.Customer.Commands;
using Invento.Application.Features.Customers.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;

namespace Invento.Application.Features.Customers.Commands
{
    public class CreateCustomerCommandHandler
        : ICommandHandler<CreateCustomerCommand, ApiResponse<CustomerDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly ICacheVersionService _cacheVersionService;

        public CreateCustomerCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            ICacheVersionService cacheVersionService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _cacheVersionService = cacheVersionService;
        }

        public async Task<ApiResponse<CustomerDto>> Handle(
                CreateCustomerCommand request,
                CancellationToken cancellationToken)
        {
            var tenantId = _currentTenant.TenantId;
         
            var customer = new Invento.Domain.Entities.Customer
            {
                TenantId = _currentTenant.TenantId,
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address
            };

            await _context.Customers.AddAsync(
                customer,
                cancellationToken);

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
                    "Customer created successfully"
                );
        }
    }
}