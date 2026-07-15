using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Common.Extensions;
using Invento.Application.Features.Customers.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Customers.Commands
{
    public class RestoreCustomerCommandHandler
        : ICommandHandler<RestoreCustomerCommand, ApiResponse<CustomerDeleteDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly ICacheVersionService _cacheVersionService;

        public RestoreCustomerCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            ICacheVersionService cacheVersionService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _cacheVersionService = cacheVersionService;
        }

        public async Task<ApiResponse<CustomerDeleteDto>> Handle(
            RestoreCustomerCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentTenant.TenantId;

            var customer = await _context.Customers
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x =>
                    x.Id == request.Id &&
                    x.TenantId == _currentTenant.TenantId &&
                    x.IsDeleted,
                    cancellationToken);

            if (customer is null)
            {
                return ApiResponse<CustomerDeleteDto>
                    .FailureResponse(
                        new List<string>
                        {
                            "Hidden customer not found"
                        }
                    );
            }

            customer.IsDeleted = false;

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
                    "Customer restored successfully"
                );
        }
    }
}