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
    public class DeleteCustomerCommandHandler
        : ICommandHandler<DeleteCustomerCommand, ApiResponse<CustomerDeleteDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly ICacheVersionService _cacheVersionService;

        public DeleteCustomerCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            ICacheVersionService cacheVersionService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _cacheVersionService = cacheVersionService;
        }

        public async Task<ApiResponse<CustomerDeleteDto>> Handle(
            DeleteCustomerCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentTenant.TenantId;

            var customer = await _context.Customers
                .FirstOrDefaultAsync(x =>
                    x.Id == request.Id &&
                    x.TenantId == _currentTenant.TenantId &&
                    !x.IsDeleted,
                    cancellationToken);

            if (customer is null)
            {
                return ApiResponse<CustomerDeleteDto>
                    .FailureResponse(
                        new List<string>
                        {
                            "Customer not found"
                        });
            }

            customer.IsDeleted = true;

            await _context.SaveChangesAsync(cancellationToken);

            await _cacheVersionService.InvalidateAsync(
                    tenantId,
                    CacheGroups.Customers,
                    CacheGroups.Receivables,
                    CacheGroups.Reports,
                    CacheGroups.Dashboard);

            return ApiResponse<CustomerDeleteDto>
                .SuccessResponse(
                    new CustomerDeleteDto
                    {
                        Id = customer.Id,
                        Name = customer.Name,
                        IsDeleted = customer.IsDeleted
                    },
                    "Customer hidden successfully"
                );
        }
    }
}