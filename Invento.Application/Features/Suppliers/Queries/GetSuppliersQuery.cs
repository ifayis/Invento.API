using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Suppliers.DTOs;

namespace Invento.Application.Features.Suppliers.Queries
{
    public class GetSuppliersQuery
        : IQuery<ApiResponse<PagedResponse<SupplierDto>>>
    {
        public string? Search { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}