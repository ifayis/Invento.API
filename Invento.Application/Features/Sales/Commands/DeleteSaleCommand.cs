using Invento.Application.Abstractions;
using Invento.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Sales.Command
{
    public class DeleteSaleCommand
        : ICommand<ApiResponse<Guid>>
    {
        public Guid Id { get; set; }
    }
}
