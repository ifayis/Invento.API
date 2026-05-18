using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Company.Commands;

public class UpdateCompanyProfileCommandHandler
    : ICommandHandler<
        UpdateCompanyProfileCommand,
        ApiResponse<Guid>>
{
    private readonly IApplicationDbContext
        _context;

    private readonly ICurrentTenantService
        _currentTenant;

    public UpdateCompanyProfileCommandHandler(
        IApplicationDbContext context,
        ICurrentTenantService currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<ApiResponse<Guid>>
        Handle(
            UpdateCompanyProfileCommand request,
            CancellationToken cancellationToken)
    {
        var tenant =
            await _context.Tenants
            .FirstOrDefaultAsync(x =>
                x.Id ==
                _currentTenant.TenantId
                && !x.IsDeleted,
                cancellationToken);

        if (tenant is null)
        {
            return ApiResponse<Guid>
                .FailureResponse(
                    new List<string>
                    {
                        "Company not found"
                    });
        }

        tenant.Name = request.Name;
        tenant.Email = request.Email;
        tenant.PhoneNumber =
            request.PhoneNumber;
        tenant.Address =
            request.Address;
        tenant.LogoUrl =
            request.LogoUrl;
        tenant.TaxNumber =
            request.TaxNumber;
        tenant.Website =
            request.Website;

        await _context.SaveChangesAsync(
            cancellationToken);

        return ApiResponse<Guid>
            .SuccessResponse(
                tenant.Id,
                "Company updated successfully");
    }
}