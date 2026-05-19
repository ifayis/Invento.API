using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Products.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Products.Commands
{
    public class DeleteProductCommand
        : ICommand<ApiResponse<ProductDto>>
    {
        public Guid Id { get; set; }
    }
}
