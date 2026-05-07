using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }

        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "Admin";

        public DateTime CreatedAt { get; set; }
    }
}
