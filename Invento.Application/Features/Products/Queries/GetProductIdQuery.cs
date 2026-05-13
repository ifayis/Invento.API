using Invento.Application.Abstractions;
using Invento.Application.Features.Products.DTOs;
using Invento.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Products.Queries
{
    public class GetProductByIdQuery
        : IQuery<ApiResponse<ProductDto>>
    {
        public Guid Id { get; set; }
    }
}
