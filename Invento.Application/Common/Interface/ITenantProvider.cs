using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Common.Interface
{
    public interface ITenantProvider
    {
        Guid GetTenantId();
        Guid GetUserId();
    }
}
