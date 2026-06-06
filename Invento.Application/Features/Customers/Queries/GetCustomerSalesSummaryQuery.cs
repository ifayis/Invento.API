using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Customers.DTOs;

namespace Invento.Application.Features.Customers.Queries
{
    public class GetCustomerSalesSummaryQuery
        : IQuery<ApiResponse<CustomerSalesSummaryDto>>
    {
        public Guid CustomerId { get; set; }
    }
}