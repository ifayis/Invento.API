using Invento.Application.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Infrastructure.Services
{
    public class TenantProvider : ITenantProvider
    {
        private Guid _tenantId;
        private Guid _userId;

        public void SetTenantId(Guid TenantId) => _tenantId = TenantId;
        public void SetUserId(Guid UserId) => _userId = UserId;

        public Guid GetTenantId() => _tenantId;
        public Guid GetUserId() => _userId;
    }
}
