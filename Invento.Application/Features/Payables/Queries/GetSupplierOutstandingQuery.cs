using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Payables.DTOs;

namespace Invento.Application.Features.Payables.Queries
{
    public class GetSupplierOutstandingQuery
        : IQuery<ApiResponse<List<SupplierOutstandingDto>>>
    {
    }
}