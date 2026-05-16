using Invento.Application.Abstractions;
using Invento.Application.Features.Profit.DTOs;
using Invento.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Profit.Queries
{
    public class GetNetProfitQuery
        : IQuery<ApiResponse<ProfitSummaryDto>>
    {
    }
}
