using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Reports.DTOs;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetCustomerSalesReportQuery
        : IQuery<ApiResponse<List<CustomerSalesReportDto>>>
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}