using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Suppliers.DTOs;

namespace Invento.Application.Features.Suppliers.Queries
{
    public class GetSupplierByIdQuery
        : IQuery<ApiResponse<SupplierDto>>
    {
        public Guid Id { get; set; }
    }
}