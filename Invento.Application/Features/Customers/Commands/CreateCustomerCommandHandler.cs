using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Customer.Commands;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Customers.Commands;

public class CreateCustomerCommandHandler
    : ICommandHandler<
        CreateCustomerCommand,
        ApiResponse<Guid>>
{
    private readonly IApplicationDbContext
        _context;

    private readonly ICurrentTenantService
        _currentTenant;

    public CreateCustomerCommandHandler(
        IApplicationDbContext context,
        ICurrentTenantService currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<ApiResponse<Guid>>
        Handle(
            CreateCustomerCommand request,
            CancellationToken cancellationToken)
    {
        var customer = new Invento.Domain.Entities.Customer
        {
            TenantId =
                _currentTenant.TenantId,

            Name = request.Name,

            Email = request.Email,

            PhoneNumber =
                request.PhoneNumber,

            Address = request.Address
        };

        await _context.Customers.AddAsync(
            customer,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        return ApiResponse<Guid>
            .SuccessResponse(
                customer.Id,
                "Customer created successfully");
    }
}