using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Balance.DTOs;

namespace Invento.Application.Features.Balance.Queries
{
    public class GetBalanceDashboardQuery
        : IQuery<ApiResponse<BalanceDashboardDto>>
    {
    }
}