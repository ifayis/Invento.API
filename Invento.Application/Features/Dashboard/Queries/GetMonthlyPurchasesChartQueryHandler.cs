using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Dashboard.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Dashboard.Queries
{
    public class GetMonthlyPurchasesChartQueryHandler
        : IQueryHandler<
            GetMonthlyPurchasesChartQuery,
            ApiResponse<List<MonthlyChartDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetMonthlyPurchasesChartQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<MonthlyChartDto>>> Handle(
            GetMonthlyPurchasesChartQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
                SELECT
                    YEAR(PurchaseDate) AS Year,
                    MONTH(PurchaseDate) AS Month,
                    SUM(TotalAmount) AS Amount
                FROM Purchases
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                GROUP BY
                    YEAR(PurchaseDate),
                    MONTH(PurchaseDate)
                ORDER BY
                    Year,
                    Month";

            var result =
                (await connection.QueryAsync<MonthlyChartDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId
                    }))
                .ToList();

            return ApiResponse<List<MonthlyChartDto>>
                .SuccessResponse(result);
        }
    }
}