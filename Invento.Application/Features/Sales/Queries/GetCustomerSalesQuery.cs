using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Sales.DTOs;

namespace Invento.Application.Features.Sales.Queries
{
    public class GetCustomerSalesQuery
        : IQuery<ApiResponse<CustomerSalesSummaryDto>>
    {
        public Guid CustomerId { get; set; }
    }
}