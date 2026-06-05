using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Targets.DTOs;

namespace Invento.Application.Features.Targets.Commands
{
    public class UpdateTenantTargetsCommand
        : ICommand<ApiResponse<TenantTargetDto>>
    {
        public decimal MonthlySalesTarget { get; set; }

        public decimal MonthlyProfitTarget { get; set; }
    }
}
