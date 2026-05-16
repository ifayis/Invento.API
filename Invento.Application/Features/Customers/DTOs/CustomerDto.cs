using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Customer.DTOs
{
    public class CustomerDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
            = string.Empty;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
