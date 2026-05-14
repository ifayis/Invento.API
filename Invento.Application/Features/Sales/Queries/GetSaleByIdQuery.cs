using Invento.Application.Abstractions;
using Invento.Application.Features.Sales.DTOs;
using Invento.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Sales.Queries
{
    public class GetSaleByIdQuery
        : IQuery<ApiResponse<SaleDetailsDto>>
    {
        public Guid Id { get; set; }
    }
}
