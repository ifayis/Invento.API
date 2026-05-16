using Invento.Application.Abstractions;
using Invento.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Profit.Queries
{
    public class GetProductProfitQuery
        : IQuery<ApiResponse<decimal>>
    {
        public Guid ProductId { get; set; }
    }
}
