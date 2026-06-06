using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Customers.DTOs;

namespace Invento.Application.Features.Customer.Commands
{
    public class CreateCustomerCommand : ICommand<ApiResponse<CustomerDto>>
    {
        public string Name { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}
