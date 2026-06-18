using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Dashboard.DTOs;

namespace Invento.Application.Features.Dashboard.Queries
{
    public class GetMonthlyPurchasesChartQuery
    : IQuery<ApiResponse<List<MonthlyChartDto>>>
    {
    }
}
