using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Purchases.DTOs;

namespace Invento.Application.Features.Purchases.Commands
{
    public class RestorePurchaseCommand
        : ICommand<ApiResponse<PurchaseDto>>
    {
        public Guid Id { get; set; }
    }
}