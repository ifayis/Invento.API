using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Suppliers.DTOs;

namespace Invento.Application.Features.Suppliers.Commands
{
    public class CreateSupplierCommand
        : ICommand<ApiResponse<SupplierDto>>
    {
        public string Name { get; set; }
            = string.Empty;

        public string? ContactPerson { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? TaxRegistrationNumber { get; set; }
    }
}