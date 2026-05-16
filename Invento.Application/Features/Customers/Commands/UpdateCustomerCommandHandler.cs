using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Customer.Commands;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Customers.Commands;

public class UpdateCustomerCommandHandler
    : ICommandHandler<
        UpdateCustomerCommand,
        ApiResponse<Guid>>
{
    private readonly IApplicationDbContext
        _context;

    private readonly ICurrentTenantService
        _currentTenant;

    public UpdateCustomerCommandHandler(
        IApplicationDbContext context,
        ICurrentTenantService currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<ApiResponse<Guid>>
        Handle(
            UpdateCustomerCommand request,
            CancellationToken cancellationToken)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(x =>
                x.Id == request.Id
                && x.TenantId
                    == _currentTenant.TenantId
                && !x.IsDeleted,
                cancellationToken);

        if (customer is null)
        {
            return ApiResponse<Guid>
                .FailureResponse(
                    new List<string>
                    {
                        "Customer not found"
                    });
        }

        customer.Name = request.Name;
        customer.Email = request.Email;
        customer.PhoneNumber =
            request.PhoneNumber;
        customer.Address =
            request.Address;

        await _context.SaveChangesAsync(
            cancellationToken);

        return ApiResponse<Guid>
            .SuccessResponse(
                customer.Id,
                "Customer updated successfully");
    }
}