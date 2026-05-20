using Invento.Application.Abstractions;
using Invento.Application.Features.Profit.DTOs;
using Invento.Application.Common;

namespace Invento.Application.Features.Profit.Queries
{
    public class GetDashboardSummaryQuery
        : IQuery<ApiResponse<DashboardSummaryDto>>
    {
    }
}
