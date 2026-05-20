using Invento.Application.Common;
using Invento.Application.Abstractions;
using Invento.Application.Features.Targets.DTOs;

namespace Invento.Application.Features.Targets.Queries
{
    public class GetTenantTargetsQuery
        : IQuery<ApiResponse<TenantTargetDto>>
    {
    }
}
