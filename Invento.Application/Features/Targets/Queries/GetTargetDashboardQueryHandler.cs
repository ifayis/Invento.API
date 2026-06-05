using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Targets.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Targets.Queries
{
    public class GetTargetDashboardQueryHandler
        : IQueryHandler<
            GetTargetDashboardQuery,
            ApiResponse<DashboardTargetDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetTargetDashboardQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<DashboardTargetDto>> Handle(
            GetTargetDashboardQuery request,
            CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
            SELECT
                ts.MonthlySalesTarget,

                ISNULL
                (
                    (
                        SELECT SUM(s.TotalAmount)
                        FROM Sales s
                        WHERE
                            s.TenantId = @TenantId
                            AND s.IsDeleted = 0
                            AND s.SaleDate >= DATEFROMPARTS
                            (
                                YEAR(GETUTCDATE()),
                                MONTH(GETUTCDATE()),
                                1
                            )
                            AND s.SaleDate <
                                DATEADD
                                (
                                    MONTH,
                                    1,
                                    DATEFROMPARTS
                                    (
                                        YEAR(GETUTCDATE()),
                                        MONTH(GETUTCDATE()),
                                        1
                                    )
                                )
                    ),
                    0
                ) AS CurrentMonthSales,

                ts.MonthlyProfitTarget,

                ISNULL
                (
                    (
                        SELECT SUM(s.ProfitAmount)
                        FROM Sales s
                        WHERE
                            s.TenantId = @TenantId
                            AND s.IsDeleted = 0
                            AND s.SaleDate >= DATEFROMPARTS
                            (
                                YEAR(GETUTCDATE()),
                                MONTH(GETUTCDATE()),
                                1
                            )
                            AND s.SaleDate <
                                DATEADD
                                (
                                    MONTH,
                                    1,
                                    DATEFROMPARTS
                                    (
                                        YEAR(GETUTCDATE()),
                                        MONTH(GETUTCDATE()),
                                        1
                                    )
                                )
                    ),
                    0
                ) AS CurrentMonthProfit,

                (
                    SELECT COUNT(*)
                    FROM Products p
                    WHERE
                        p.TenantId = @TenantId
                        AND p.IsDeleted = 0
                        AND p.CurrentStock <= p.LowStockThreshold
                ) AS LowStockProducts,

                (
                    SELECT COUNT(*)
                    FROM Products p
                    WHERE
                        p.TenantId = @TenantId
                        AND p.IsDeleted = 0
                        AND p.CurrentStock <= (p.LowStockThreshold / 2)
                ) AS CriticalStockProducts

            FROM TenantSettings ts

            WHERE
                ts.TenantId = @TenantId
                AND ts.IsDeleted = 0
            ";
            var dashboard =
                await connection.QueryFirstOrDefaultAsync<DashboardTargetDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId
                    });

            if (dashboard is null)
            {
                dashboard = new DashboardTargetDto
                {
                    MonthlySalesTarget = 0,
                    CurrentMonthSales = 0,
                    SalesAchievementPercentage = 0,

                    MonthlyProfitTarget = 0,
                    CurrentMonthProfit = 0,
                    ProfitAchievementPercentage = 0,

                    LowStockProducts = 0,
                    CriticalStockProducts = 0
                };
            }

            dashboard.SalesAchievementPercentage =
                dashboard.MonthlySalesTarget == 0
                    ? 0
                    : Math.Round(
                        (dashboard.CurrentMonthSales
                         / dashboard.MonthlySalesTarget) * 100,
                        2);

            dashboard.ProfitAchievementPercentage =
                dashboard.MonthlyProfitTarget == 0
                    ? 0
                    : Math.Round(
                        (dashboard.CurrentMonthProfit
                         / dashboard.MonthlyProfitTarget) * 100,
                        2);

            return ApiResponse<DashboardTargetDto>
                .SuccessResponse(dashboard);
        }
    }
}