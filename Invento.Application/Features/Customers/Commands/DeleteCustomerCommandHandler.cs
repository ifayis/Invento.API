using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Customer.Commands;
using Invento.Application.Features.Customer.DTOs;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Customers.Commands
{
    public class DeleteCustomerCommandHandler
        : ICommandHandler<DeleteCustomerCommand, ApiResponse<CustomerDto>>
    {
        private readonly IApplicationDbContext _context;

        private readonly ICurrentTenantService _currentTenant;

        public DeleteCustomerCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant)
        {
            _context = context;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<CustomerDto>> Handle(
                DeleteCustomerCommand request,
                CancellationToken cancellationToken)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x =>
                    x.Id == request.Id
                    && x.TenantId == _currentTenant.TenantId
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

            customer.IsDeleted = true;

            await _context.SaveChangesAsync(
                cancellationToken);

            return ApiResponse<CustomerDto>
                .SuccessResponse(
                    new CustomerDto
                    {
                        Name = customer.Name
                    },
                    "Customer deleted successfully"
                );
        }
    }
}