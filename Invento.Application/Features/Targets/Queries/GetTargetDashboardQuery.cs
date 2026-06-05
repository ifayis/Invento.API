using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Targets.DTOs;

namespace Invento.Application.Features.Targets.Queries
{
    public class GetTargetDashboardQuery
        : IQuery<ApiResponse<DashboardTargetDto>>
    {
    }
}