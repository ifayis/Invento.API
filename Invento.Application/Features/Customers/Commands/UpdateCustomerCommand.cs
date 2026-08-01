using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Customers.DTOs;
using System.Text.Json.Serialization;

namespace Invento.Application.Features.Customer.Commands
{
    public class UpdateCustomerCommand
        : ICommand<ApiResponse<CustomerDto>>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}
