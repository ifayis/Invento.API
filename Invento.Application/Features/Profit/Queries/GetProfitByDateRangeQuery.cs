using Invento.Application.Common;
using Invento.Application.Abstractions;
using Invento.Application.Features.Profit.DTOs;

namespace Invento.Application.Features.Profit.Queries
{
    public class GetProfitByDateRangeQuery
        : IQuery<ApiResponse<ProfitSummaryDto>>
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
