using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Suppliers.DTOs;

namespace Invento.Application.Features.Suppliers.Queries
{
    public class GetSupplierLedgerQuery
        : IQuery<ApiResponse<SupplierLedgerDto>>
    {
        public Guid SupplierId { get; set; }
    }
}