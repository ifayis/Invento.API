using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Purchases.DTOs;

namespace Invento.Application.Features.Purchases.Commands
{
    public class CreatePurchaseCommand
        : ICommand<ApiResponse<PurchaseDetailsDto>>
    {
        public Guid SupplierId { get; set; }

        public DateTime PurchaseDate { get; set; }

        public decimal DiscountAmount { get; set; }

        public List<CreatePurchaseItemDto> Items { get; set; }
            = new();
    }
}