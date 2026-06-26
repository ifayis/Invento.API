using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Sales.DTOs;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.Sales.Queries;

public class GetSalesQuery
    : IQuery<ApiResponse<PagedResponse<SaleDto>>>
{
    public string? Search { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}