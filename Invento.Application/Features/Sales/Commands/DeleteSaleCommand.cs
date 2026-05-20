using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Sales.DTOs;

namespace Invento.Application.Features.Sales.Command
{
    public class DeleteSaleCommand
        : ICommand<ApiResponse<SaleDto>>
    {
        public Guid Id { get; set; }
    }
}
