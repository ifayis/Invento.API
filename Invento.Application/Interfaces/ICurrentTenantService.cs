using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Interfaces
{
    public interface ICurrentTenantService
    {
        Guid TenantId { get; }
    }
}
