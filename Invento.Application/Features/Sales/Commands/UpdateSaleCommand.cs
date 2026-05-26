using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Sales.DTOs;

namespace Invento.Application.Features.Sales.Command
{
    public class UpdateSaleCommand
        : ICommand<ApiResponse<SaleDto>>
    {
        public Guid Id { get; set; }

        public Guid? CustomerId { get; set; }

        public DateTime SaleDate { get; set; }

        public decimal DiscountAmount { get; set; }

        public List<CreateSaleItemDto> Items { get; set; } = new();
    }
}
