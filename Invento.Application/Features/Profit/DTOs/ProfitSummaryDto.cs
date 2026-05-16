using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Profit.DTOs
{
    public class ProfitSummaryDto
    {
        public decimal TotalRevenue { get; set; }

        public decimal TotalProfit { get; set; }

        public int TotalSales { get; set; }
    }
}
