using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Company.DTOs;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Company.Commands
{
    public class UpdateCompanyProfileCommandHandler
        : ICommandHandler<UpdateCompanyProfileCommand, ApiResponse<CompanyProfileDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;

        public UpdateCompanyProfileCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant)
        {
            _context = context;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<CompanyProfileDto>> Handle(
                UpdateCompanyProfileCommand request,
                CancellationToken cancellationToken)
        {
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(x =>
                    x.Id ==
                    _currentTenant.TenantId
                    && !x.IsDeleted,
                    cancellationToken
                );

            if (tenant is null)
            {
                return ApiResponse<CompanyProfileDto>
                    .FailureResponse(
                        new List<string>
                        {
                        "Company not found"
                        }
                    );
            }

            tenant.CompanyName = request.CompanyName;
            tenant.PhoneNumber = request.PhoneNumber;
            tenant.Address = request.Address;
            tenant.LogoUrl = request.LogoUrl;
            tenant.TaxNumber = request.TaxNumber;
            tenant.Website = request.Website;

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<CompanyProfileDto>
                .SuccessResponse(
                    new CompanyProfileDto
                    {
                        Id = tenant.Id,
                        CompanyName = tenant.CompanyName,
                        PhoneNumber = tenant.PhoneNumber,
                        Address = tenant.Address,
                        LogoUrl = tenant.LogoUrl,
                        TaxNumber = tenant.TaxNumber,
                        Website = tenant.Website
                    },
                    "Company updated successfully"
                );
        }
    }
}