using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Suppliers.DTOs;
using System.Text.Json.Serialization;

namespace Invento.Application.Features.Suppliers.Commands
{
    public class UpdateSupplierCommand
        : ICommand<ApiResponse<SupplierDto>>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string Name { get; set; }
            = string.Empty;

        public string? ContactPerson { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? TaxRegistrationNumber { get; set; }
    }
}