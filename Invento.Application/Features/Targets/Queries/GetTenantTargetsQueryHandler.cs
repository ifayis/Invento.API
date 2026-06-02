using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Targets.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Targets.Queries
{
    public class GetTenantTargetsQueryHandler
        : IQueryHandler<
            GetTenantTargetsQuery,
            ApiResponse<TenantTargetDto>>
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
            using var connection =
                _connectionFactory.CreateConnection();

            var targetSql = @"
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

            var result =
                await connection.QueryFirstOrDefaultAsync<TenantTargetDto>(
                    targetSql,
                    new
                    {
                        TenantId = _currentTenant.TenantId
                    });

            if (result is null)
            {
                result = new TenantTargetDto();
            }

            var firstDayOfMonth =
                new DateTime(
                    DateTime.UtcNow.Year,
                    DateTime.UtcNow.Month,
                    1);

            var salesSql = @"
                SELECT
                    ISNULL(SUM(TotalAmount),0)
                FROM Sales
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                    AND SaleDate >= @FirstDayOfMonth
            ";

            var profitSql = @"
                SELECT
                    ISNULL(SUM(ProfitAmount),0)
                FROM Sales
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                    AND SaleDate >= @FirstDayOfMonth
            ";

            result.CurrentMonthSales =
                await connection.ExecuteScalarAsync<decimal>(
                    salesSql,
                    new
                    {
                        TenantId = _currentTenant.TenantId,
                        FirstDayOfMonth = firstDayOfMonth
                    });

            result.CurrentMonthProfit =
                await connection.ExecuteScalarAsync<decimal>(
                    profitSql,
                    new
                    {
                        TenantId = _currentTenant.TenantId,
                        FirstDayOfMonth = firstDayOfMonth
                    });

            result.SalesAchievementPercentage =
                result.MonthlySalesTarget == 0
                    ? 0
                    : Math.Round(
                        (result.CurrentMonthSales
                        / result.MonthlySalesTarget) * 100,
                        2);

            result.ProfitAchievementPercentage =
                result.MonthlyProfitTarget == 0
                    ? 0
                    : Math.Round(
                        (result.CurrentMonthProfit
                        / result.MonthlyProfitTarget) * 100,
                        2);

            return ApiResponse<TenantTargetDto>
                .SuccessResponse(result);
        }
    }
}