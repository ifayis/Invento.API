using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Dashboard.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Enums;

namespace Invento.Application.Features.Dashboard.Queries
{
    public class GetDashboardSummaryQueryHandler
        : IQueryHandler<
            GetDashboardSummaryQuery,
            ApiResponse<DashboardSummaryDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetDashboardSummaryQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<DashboardSummaryDto>> Handle(
            GetDashboardSummaryQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            const string sql = """
                SELECT
                    ISNULL(
                        (
                            SELECT SUM(TotalAmount)
                            FROM Sales
                            WHERE
                                TenantId = @TenantId
                                AND IsDeleted = 0
                        ),
                        0
                    ) AS TotalSales,

                    ISNULL(
                        (
                            SELECT SUM(TotalAmount)
                            FROM Purchases
                            WHERE
                                TenantId = @TenantId
                                AND IsDeleted = 0
                        ),
                        0
                    ) AS TotalPurchases,

                    (
                        ISNULL(
                            (
                                SELECT SUM(Amount)
                                FROM CashTransactions
                                WHERE
                                    TenantId = @TenantId
                                    AND IsDeleted = 0
                                    AND TransactionType
                                        IN (@Sale, @ManualIncome)
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
                                    AND TransactionType
                                        IN (@Purchase, @ManualExpense)
                            ),
                            0
                        )
                    ) AS CurrentBalance,

                    (
                        SELECT COUNT(*)
                        FROM Products
                        WHERE
                            TenantId = @TenantId
                            AND IsDeleted = 0
                    ) AS TotalProducts,

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
                        FROM Customers
                        WHERE
                            TenantId = @TenantId
                            AND IsDeleted = 0
                    ) AS TotalCustomers,

                    (
                        SELECT COUNT(*)
                        FROM Suppliers
                        WHERE
                            TenantId = @TenantId
                            AND IsDeleted = 0
                    ) AS TotalSuppliers;
                """;

            var command =
                new CommandDefinition(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId,

                        Sale =
                            (int)CashTransactionType.Sale,

                        Purchase =
                            (int)CashTransactionType.Purchase,

                        ManualIncome =
                            (int)CashTransactionType.ManualIncome,

                        ManualExpense =
                            (int)CashTransactionType.ManualExpense
                    },
                    cancellationToken:
                        cancellationToken);

            var result =
                await connection
                    .QueryFirstAsync<DashboardSummaryDto>(
                        command);

            return ApiResponse<DashboardSummaryDto>
                .SuccessResponse(result);
        }
    }
}