using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Sales.DTOs;

namespace Invento.Application.Features.Sales.Commands
{
    public class RestoreSaleCommand
        : ICommand<ApiResponse<DeleteSaleDto>>
    {
        public Guid Id { get; set; }
    }
}