using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Domain.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string CompanyCode { get; set; } = "";

        public string? BusinessPurpose { get; set; }
        public string? LogoUrl { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
