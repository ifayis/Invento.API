using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Common.Extensions;
using Invento.Application.Features.Targets.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Targets.Commands
{
    public class UpdateTenantTargetsCommandHandler
        : ICommandHandler<UpdateTenantTargetsCommand, ApiResponse<TenantTargetDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly ICacheVersionService _cacheVersionService;

        public UpdateTenantTargetsCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            ICacheVersionService cacheVersionService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _cacheVersionService = cacheVersionService;
        }

        public async Task<ApiResponse<TenantTargetDto>> Handle(
                UpdateTenantTargetsCommand request,
                CancellationToken cancellationToken)
        {
            var tenantId = _currentTenant.TenantId;

            var settings = await _context.TenantSettings
                .FirstOrDefaultAsync(x =>
                    x.TenantId
                        == _currentTenant.TenantId,
                    cancellationToken);

            if (settings is null)
            {
                settings = new TenantSettings
                {
                    TenantId = _currentTenant.TenantId
                };

                await _context.TenantSettings
                    .AddAsync(
                        settings,
                        cancellationToken
                    );
            }

            settings.MonthlySalesTarget = request.MonthlySalesTarget;

            settings.MonthlyProfitTarget = request.MonthlyProfitTarget;

            await _context.SaveChangesAsync(cancellationToken);

            await _cacheVersionService.InvalidateAsync(
                    tenantId,
                    CacheGroups.Targets,
                    CacheGroups.Reports,
                    CacheGroups.Dashboard);

            return ApiResponse<TenantTargetDto>
                .SuccessResponse(
                new TenantTargetDto
                {
                    MonthlyProfitTarget = settings.MonthlyProfitTarget,
                    MonthlySalesTarget = settings.MonthlySalesTarget
                },
                    "Target settings updated"
                );
        }
    }
}