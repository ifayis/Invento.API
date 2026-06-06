using Invento.Application.Abstractions;
using Invento.Application.Features.Customers.DTOs;
using Invento.Application.Common;

namespace Invento.Application.Features.Customers.Queries
{
    public class GetCustomerByIdQuery
        : IQuery<ApiResponse<CustomerDto>>
    {
        public Guid Id { get; set; }
    }
}