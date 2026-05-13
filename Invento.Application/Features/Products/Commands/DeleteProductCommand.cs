using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invento.Application.Abstractions;
using Invento.Application.Common;

namespace Invento.Application.Features.Products.Commands
{
    public class DeleteProductCommand
        : ICommand<ApiResponse<Guid>>
    {
        public Guid Id { get; set; }
    }
}
