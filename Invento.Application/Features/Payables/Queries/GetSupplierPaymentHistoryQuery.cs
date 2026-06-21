using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Payables.DTOs;

namespace Invento.Application.Features.Payables.Queries
{
    public class GetSupplierPaymentHistoryQuery
        : IQuery<ApiResponse<List<SupplierPaymentHistoryDto>>>
    {
        public Guid SupplierId { get; set; }
    }
}