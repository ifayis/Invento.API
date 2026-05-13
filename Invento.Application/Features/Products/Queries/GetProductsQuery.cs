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
    public class GetProductsQuery
        : IQuery<ApiResponse<PagedResponse<ProductDto>>>
    {
        public string? Search { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
