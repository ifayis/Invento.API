using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Purchases.DTOs;

namespace Invento.Application.Features.Purchases.Queries
{
    public class GetPurchaseByIdQuery
        : IQuery<ApiResponse<PurchaseDetailsDto>>
    {
        public Guid Id { get; set; }
    }
}