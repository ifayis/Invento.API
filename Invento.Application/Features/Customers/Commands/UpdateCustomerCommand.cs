using Invento.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invento.Application.Common;

namespace Invento.Application.Features.Customer.Commands
{
    public class UpdateCustomerCommand
        : ICommand<ApiResponse<Guid>>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
            = string.Empty;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}
