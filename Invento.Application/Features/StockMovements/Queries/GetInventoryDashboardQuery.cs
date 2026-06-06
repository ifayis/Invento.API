using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.StockMovements.DTOs;

namespace Invento.Application.Features.StockMovements.Queries
{
    public class GetInventoryDashboardQuery
        : IQuery<ApiResponse<InventoryDashboardDto>>
    {
    }
}