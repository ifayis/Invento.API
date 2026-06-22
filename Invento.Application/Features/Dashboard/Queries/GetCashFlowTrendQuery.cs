using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Dashboard.DTOs;

namespace Invento.Application.Features.Dashboard.Queries
{
    public class GetCashFlowTrendQuery
        : IQuery<ApiResponse<List<CashFlowTrendDto>>>
    {
    }
}