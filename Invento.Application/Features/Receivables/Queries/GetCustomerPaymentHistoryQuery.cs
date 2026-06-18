using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Receivables.DTOs;

namespace Invento.Application.Features.Receivables.Queries
{
    public class GetCustomerPaymentHistoryQuery
        : IQuery<ApiResponse<List<CustomerPaymentHistoryDto>>>
    {
        public Guid CustomerId { get; set; }
    }
}