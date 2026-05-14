using Invento.Application.Abstractions;
using Invento.Application.Features.StockMovements.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invento.Application.Common;

namespace Invento.Application.Features.StockMovements.Queries
{
    public class GetStockMovementByIdQuery
        : IQuery<ApiResponse<StockMovementDto>>
    {
        public Guid Id { get; set; }
    }
}
