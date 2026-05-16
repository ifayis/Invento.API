using Invento.Application.Abstractions;
using Invento.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Targets.Commands
{
    public class UpdateTenantTargetsCommand
        : ICommand<ApiResponse<Guid>>
    {
        public int LowStockThreshold { get; set; }

        public int CriticalStockThreshold { get; set; }

        public decimal MonthlySalesTarget { get; set; }

        public decimal MonthlyProfitTarget { get; set; }
    }
}
