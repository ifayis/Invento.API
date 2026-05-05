using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Common.Interface
{
    public interface  IJwtService
    {
        string GenerateToken(Guid userId, Guid tenantId, string role, string email);
    }
}
