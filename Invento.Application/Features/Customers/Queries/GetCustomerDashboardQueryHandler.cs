using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Customers.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Customers.Queries
{
    public class GetCustomerDashboardQueryHandler
        : IQueryHandler<
            GetCustomerDashboardQuery,
            ApiResponse<CustomerDashboardDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetCustomerDashboardQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<CustomerDashboardDto>> Handle(
            GetCustomerDashboardQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"

            SELECT

            (
                SELECT COUNT(*)
                FROM Customers
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
            ) AS TotalCustomers,

            (
                SELECT COUNT(DISTINCT CustomerId)
                FROM Sales
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                    AND CustomerId IS NOT NULL
            ) AS ActiveCustomers,

            (
                SELECT COUNT(*)
                FROM Customers c
                WHERE
                    c.TenantId = @TenantId
                    AND c.IsDeleted = 0
                    AND NOT EXISTS
                    (
                        SELECT 1
                        FROM Sales s
                        WHERE
                            s.CustomerId = c.Id
                            AND s.IsDeleted = 0
                    )
            ) AS InactiveCustomers,

            (
                SELECT COUNT(*)
                FROM Customers
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                    AND CreatedAt >= DATEFROMPARTS
                    (
                        YEAR(GETUTCDATE()),
                        MONTH(GETUTCDATE()),
                        1
                    )
            ) AS NewCustomersThisMonth,

            ISNULL
            (
                (
                    SELECT TOP 1
                        SUM(s.TotalAmount)
                    FROM Sales s
                    WHERE
                        s.TenantId = @TenantId
                        AND s.IsDeleted = 0
                        AND s.CustomerId IS NOT NULL
                    GROUP BY s.CustomerId
                    ORDER BY SUM(s.TotalAmount) DESC
                ),
                0
            ) AS TopCustomerRevenue,

            ISNULL
            (
                (
                    SELECT TOP 1
                        SUM(s.ProfitAmount)
                    FROM Sales s
                    WHERE
                        s.TenantId = @TenantId
                        AND s.IsDeleted = 0
                        AND s.CustomerId IS NOT NULL
                    GROUP BY s.CustomerId
                    ORDER BY SUM(s.ProfitAmount) DESC
                ),
                0
            ) AS TopCustomerProfit
            ";

            var dashboard =
                await connection.QueryFirstOrDefaultAsync<
                    CustomerDashboardDto>(
                    sql,
                    new
                    {
                        TenantId =
                            _currentTenant.TenantId
                    });

            dashboard ??=
                new CustomerDashboardDto();

            return ApiResponse<CustomerDashboardDto>
                .SuccessResponse(dashboard);
        }
    }
}