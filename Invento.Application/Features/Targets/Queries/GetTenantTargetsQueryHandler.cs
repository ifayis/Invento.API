using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Targets.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Targets.Queries
{
    public class GetTenantTargetsQueryHandler
        : IQueryHandler<GetTenantTargetsQuery, ApiResponse<TenantTargetDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetTenantTargetsQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<TenantTargetDto>> Handle(
                GetTenantTargetsQuery request,
                CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateConnection();

                var sql = @"
                SELECT
                    LowStockThreshold,
                    CriticalStockThreshold,
                    MonthlySalesTarget,
                    MonthlyProfitTarget

                FROM TenantSettings

                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                ";

            var result = await connection.QueryFirstOrDefaultAsync<TenantTargetDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId
                    }
            );

            result ??= new TenantTargetDto();

            return ApiResponse<TenantTargetDto>
                .SuccessResponse(result);
        }
    }
}