using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Profit.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Profit.Queries
{
    public class GetProfitByDateRangeQueryHandler
        : IQueryHandler<GetProfitByDateRangeQuery,  ApiResponse<ProfitSummaryDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetProfitByDateRangeQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<ProfitSummaryDto>> Handle(
                GetProfitByDateRangeQuery request,
                CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
            SELECT
                ISNULL(SUM(TotalAmount), 0)
                    AS TotalRevenue,

                ISNULL(SUM(ProfitAmount), 0)
                    AS TotalProfit,

                COUNT(*) AS TotalSales

            FROM Sales

            WHERE
                IsDeleted = 0
                AND TenantId = @TenantId
            AND
            (
                @FromDate IS NULL
                OR SaleDate >= @FromDate
            )

            AND
            (
                @ToDate IS NULL
                OR SaleDate <= @ToDate
            )            
            ";

            var result = await connection
                .QueryFirstAsync<ProfitSummaryDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId,
                        request.FromDate,
                        request.ToDate
                    }
                );

            return ApiResponse<ProfitSummaryDto>
                .SuccessResponse(result);
        }
    }
}