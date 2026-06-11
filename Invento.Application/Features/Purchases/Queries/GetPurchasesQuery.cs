using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Purchases.DTOs;

namespace Invento.Application.Features.Purchases.Queries
{
    public class GetPurchasesQuery
        : IQuery<ApiResponse<PagedResponse<PurchaseDto>>>
    {
        public string? Search { get; set; }

        public Guid? SupplierId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}