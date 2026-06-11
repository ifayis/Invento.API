using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Suppliers.DTOs;

namespace Invento.Application.Features.Suppliers.Commands
{
    public class RestoreSupplierCommand
        : ICommand<ApiResponse<SupplierDto>>
    {
        public Guid Id { get; set; }
    }
}