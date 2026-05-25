using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Customers.DTOs;

namespace Invento.Application.Features.Customers.Commands
{
    public class RestoreCustomerCommand
            : ICommand<ApiResponse<CustomerDeleteDto>>
    {
        public Guid Id { get; set; }
    }
}
