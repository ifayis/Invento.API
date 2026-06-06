using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Customers.DTOs;

namespace Invento.Application.Features.Customers.Queries
{
    public class GetTopCustomersQuery
        : IQuery<ApiResponse<List<CustomerSalesSummaryDto>>>
    {
        public int Top { get; set; } = 10;
    }
}