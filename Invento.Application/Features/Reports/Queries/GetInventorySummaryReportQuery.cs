using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Reports.DTOs;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetInventorySummaryReportQuery
    : IQuery<ApiResponse<InventorySummaryReportDto>>
    {
    }
}