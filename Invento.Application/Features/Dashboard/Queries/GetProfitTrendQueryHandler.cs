using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Dashboard.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Dashboard.Queries
{
    public class GetProfitTrendQueryHandler
        : IQueryHandler<
            GetProfitTrendQuery,
            ApiResponse<List<ProfitTrendDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetProfitTrendQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<ProfitTrendDto>>> Handle(
            GetProfitTrendQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
            SELECT
                FORMAT(
                    SaleDate,
                    'MMM yyyy'
                ) AS Month,

                SUM(TotalAmount)
                    AS Revenue,

                SUM(ProfitAmount)
                    AS Profit,

                CASE
                    WHEN SUM(TotalAmount) = 0
                    THEN 0
                    ELSE
                    (
                        SUM(ProfitAmount) * 100.0
                        /
                        SUM(TotalAmount)
                    )
                END AS ProfitMargin

            FROM Sales

            WHERE
                TenantId = @TenantId
                AND IsDeleted = 0

            GROUP BY
                YEAR(SaleDate),
                MONTH(SaleDate),
                FORMAT(SaleDate,'MMM yyyy')

            ORDER BY
                YEAR(SaleDate),
                MONTH(SaleDate);
            ";

            var result =
                (await connection.QueryAsync<ProfitTrendDto>(
                    sql,
                    new
                    {
                        TenantId =
                            _currentTenant.TenantId
                    }))
                .ToList();

            return ApiResponse<List<ProfitTrendDto>>
                .SuccessResponse(result);
        }
    }
}