using Invento.Application.Abstractions;
using Invento.Application.Common;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetStockMovementHistoryQuery
    : IQuery<ApiResponse<PagedResponse<StockMovementDto>>>
    {
        public Guid? ProductId { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }

}
