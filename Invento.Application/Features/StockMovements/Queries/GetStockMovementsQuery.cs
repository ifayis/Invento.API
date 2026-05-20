using Invento.Application.Abstractions;
using Invento.Application.Features.StockMovements.DTOs;
using Invento.Application.Common;

namespace Invento.Application.Features.StockMovements.Queries
{
    public class GetStockMovementsQuery
        : IQuery<ApiResponse<PagedResponse<StockMovementDto>>>
    {
        public Guid? ProductId { get; set; }

        public string? Search { get; set; }

        public string? MovementType { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
