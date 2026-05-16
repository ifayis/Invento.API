using Invento.Application.Abstractions;
using Invento.Application.Features.Targets.DTOs;
using Invento.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Targets.Queries
{
    public class GetLowStockProductsQuery
        : IQuery<
            ApiResponse<List<StockAlertDto>>>
    {
    }
}
