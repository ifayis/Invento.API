using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Dashboard.DTOs;

namespace Invento.Application.Features.Dashboard.Queries
{
    public class GetTopSuppliersQuery
    : IQuery<ApiResponse<List<TopSupplierDto>>>
    {
        public int Count { get; set; } = 10;
    }
}
