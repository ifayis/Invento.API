using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Balance.DTOs;

namespace Invento.Application.Features.Balance.Queries
{
    public class GetCashFlowQuery
        : IQuery<ApiResponse<CashFlowDto>>
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}