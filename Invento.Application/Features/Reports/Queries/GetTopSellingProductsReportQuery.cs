using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Reports.DTOs;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetTopSellingProductsReportQuery
        : IQuery<ApiResponse<List<TopSellingProductReportDto>>>
    {
        public int Top { get; set; } = 10;

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}