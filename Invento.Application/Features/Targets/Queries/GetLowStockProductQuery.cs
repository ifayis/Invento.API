using Invento.Application.Abstractions;
using Invento.Application.Features.Targets.DTOs;
using Invento.Application.Common;

namespace Invento.Application.Features.Targets.Queries
{
    public class GetLowStockProductsQuery
        : IQuery< ApiResponse<List<StockAlertDto>>>
    {
    }
}
