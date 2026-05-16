using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Targets.Commands;

public class UpdateTenantTargetsCommandHandler
    : ICommandHandler<
        UpdateTenantTargetsCommand,
        ApiResponse<Guid>>
{
    private readonly IApplicationDbContext
        _context;

    private readonly ICurrentTenantService
        _currentTenant;

    public UpdateTenantTargetsCommandHandler(
        IApplicationDbContext context,
        ICurrentTenantService currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<ApiResponse<Guid>>
        Handle(
            UpdateTenantTargetsCommand request,
            CancellationToken cancellationToken)
    {
        var settings =
            await _context.TenantSettings
            .FirstOrDefaultAsync(x =>
                x.TenantId
                    == _currentTenant.TenantId,
                cancellationToken);

        if (settings is null)
        {
            settings = new TenantSettings
            {
                TenantId =
                    _currentTenant.TenantId
            };

            await _context.TenantSettings
                .AddAsync(
                    settings,
                    cancellationToken);
        }

        settings.LowStockThreshold =
            request.LowStockThreshold;

        settings.CriticalStockThreshold =
            request.CriticalStockThreshold;

        settings.MonthlySalesTarget =
            request.MonthlySalesTarget;

        settings.MonthlyProfitTarget =
            request.MonthlyProfitTarget;

        await _context.SaveChangesAsync(
            cancellationToken);

        return ApiResponse<Guid>
            .SuccessResponse(
                settings.Id,
                "Target settings updated");
    }
}