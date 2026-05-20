using Invento.Application.Abstractions;
using Invento.Application.Features.Sales.DTOs;
using Invento.Application.Common;

namespace Invento.Application.Features.Sales.Queries
{
    public class GetSaleByIdQuery
        : IQuery<ApiResponse<SaleDetailsDto>>
    {
        public Guid Id { get; set; }
    }
}
