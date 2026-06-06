using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Customers.DTOs;

namespace Invento.Application.Features.Customer.Commands
{
    public class DeleteCustomerCommand 
        : ICommand<ApiResponse<CustomerDeleteDto>>
    {
        public Guid Id { get; set; }
    }
}
