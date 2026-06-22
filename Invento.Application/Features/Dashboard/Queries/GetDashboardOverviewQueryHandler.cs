using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Dashboard.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Dashboard.Queries
{
    public class GetDashboardOverviewQueryHandler
        : IQueryHandler<
            GetDashboardOverviewQuery,
            ApiResponse<DashboardOverviewDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetDashboardOverviewQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<DashboardOverviewDto>> Handle(
            GetDashboardOverviewQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
            SELECT

                ISNULL(
                (
                    SELECT SUM(TotalAmount)
                    FROM Sales
                    WHERE
                        TenantId = @TenantId
                        AND IsDeleted = 0
                        AND CAST(SaleDate AS DATE) = CAST(GETUTCDATE() AS DATE)
                ),
                0
                ) AS TodaySales,

                ISNULL(
                (
                    SELECT SUM(TotalAmount)
                    FROM Sales
                    WHERE
                        TenantId = @TenantId
                        AND IsDeleted = 0
                        AND YEAR(SaleDate) = YEAR(GETUTCDATE())
                        AND MONTH(SaleDate) = MONTH(GETUTCDATE())
                ),
                0
                ) AS MonthlySales,

                ISNULL(
                (
                    SELECT SUM(ProfitAmount)
                    FROM Sales
                    WHERE
                        TenantId = @TenantId
                        AND IsDeleted = 0
                        AND YEAR(SaleDate) = YEAR(GETUTCDATE())
                        AND MONTH(SaleDate) = MONTH(GETUTCDATE())
                ),
                0
                ) AS MonthlyProfit,

                ISNULL(
                (
                    SELECT SUM(TotalAmount)
                    FROM Purchases
                    WHERE
                        TenantId = @TenantId
                        AND IsDeleted = 0
                        AND YEAR(PurchaseDate) = YEAR(GETUTCDATE())
                        AND MONTH(PurchaseDate) = MONTH(GETUTCDATE())
                ),
                0
                ) AS MonthlyPurchases,

                ISNULL(
                (
                    SELECT SUM(DueAmount)
                    FROM Sales
                    WHERE
                        TenantId = @TenantId
                        AND IsDeleted = 0
                ),
                0
                ) AS OutstandingReceivables,

                ISNULL(
                (
                    SELECT SUM(DueAmount)
                    FROM Purchases
                    WHERE
                        TenantId = @TenantId
                        AND IsDeleted = 0
                ),
                0
                ) AS OutstandingPayables,

                (
                    ISNULL(
                    (
                        SELECT SUM(Amount)
                        FROM CashTransactions
                        WHERE
                            TenantId = @TenantId
                            AND IsDeleted = 0
                            AND TransactionType IN (1,3)
                    ),
                    0
                    )
                    -
                    ISNULL(
                    (
                        SELECT SUM(Amount)
                        FROM CashTransactions
                        WHERE
                            TenantId = @TenantId
                            AND IsDeleted = 0
                            AND TransactionType IN (2,4)
                    ),
                    0
                    )
                ) AS CashBalance,

                (
                    SELECT COUNT(*)
                    FROM Products
                    WHERE
                        TenantId = @TenantId
                        AND IsDeleted = 0
                        AND CurrentStock <= LowStockThreshold
                ) AS LowStockProducts,

                (
                    SELECT COUNT(*)
                    FROM Products
                    WHERE
                        TenantId = @TenantId
                        AND IsDeleted = 0
                        AND CurrentStock <= 5
                ) AS CriticalStockProducts;
            ";

            var result =
                await connection.QueryFirstAsync<DashboardOverviewDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId
                    });

            return ApiResponse<DashboardOverviewDto>
                .SuccessResponse(result);
        }
    }
}