using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Customer.DTOs;

namespace Invento.Application.Features.Customer.Commands
{
    public class DeleteCustomerCommand 
        : ICommand<ApiResponse<CustomerDto>>
    {
        public Guid Id { get; set; }
    }
}
