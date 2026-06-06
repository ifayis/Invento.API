using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Customers.DTOs;

namespace Invento.Application.Features.Customers.Queries
{
    public class GetInactiveCustomersQuery
        : IQuery<ApiResponse<List<InactiveCustomerDto>>>
    {
        public int Days { get; set; } = 30;
    }
}