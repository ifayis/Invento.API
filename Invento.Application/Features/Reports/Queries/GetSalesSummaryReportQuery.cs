using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Reports.DTOs;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetSalesSummaryReportQuery
        : IQuery<ApiResponse<SalesSummaryReportDto>>
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}