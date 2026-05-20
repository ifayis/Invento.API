using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Sales.DTOs;

namespace Invento.Application.Features.Sales.Command
{
    public class CreateSaleCommand
        : ICommand<ApiResponse<SaleDetailsDto>>
    {
        public DateTime SaleDate { get; set; }

        public decimal DiscountAmount { get; set; }

        public List<CreateSaleItemDto> Items { get; set; } = new();
    }
}
