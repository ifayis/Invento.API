using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Sales.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Sales.Command
{
    public class DeleteSaleCommand
        : ICommand<ApiResponse<SaleDto>>
    {
        public Guid Id { get; set; }
    }
}
