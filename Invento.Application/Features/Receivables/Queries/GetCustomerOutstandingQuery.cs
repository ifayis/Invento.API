using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Receivables.DTOs;

namespace Invento.Application.Features.Receivables.Queries
{
    public class GetCustomerOutstandingQuery
        : IQuery<ApiResponse<List<CustomerOutstandingDto>>>
    {
    }
}