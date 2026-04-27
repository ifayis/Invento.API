using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Domain.Entities
{
    public abstract class Base
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
