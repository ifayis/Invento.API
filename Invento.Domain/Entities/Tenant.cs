using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invento.Shared.Common;

namespace Invento.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public string CompanyName { get; set; } = string.Empty;

        public string BusinessPurpose { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? LogoUrl { get; set; }

        public ICollection<User> Users { get; set; }
            = new List<User>();
    }
}
