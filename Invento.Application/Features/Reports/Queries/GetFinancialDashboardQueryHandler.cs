using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Reports.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Enums;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetFinancialDashboardQueryHandler
        : IQueryHandler<
            GetFinancialDashboardQuery,
            ApiResponse<FinancialDashboardDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetFinancialDashboardQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<FinancialDashboardDto>> Handle(
            GetFinancialDashboardQuery request,
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
                    ) AS TotalSalesRevenue,

                    ISNULL(
                        (
                            SELECT SUM(TotalAmount)
                            FROM Purchases
                            WHERE
                                TenantId = @TenantId
                                AND IsDeleted = 0
                        ),
                        0
                    ) AS TotalPurchaseCost,

                    ISNULL(
                        (
                            SELECT SUM(ProfitAmount)
                            FROM Sales
                            WHERE
                                TenantId = @TenantId
                                AND IsDeleted = 0
                        ),
                        0
                    ) AS TotalProfit,

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
                    ) AS CashInflow,

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
                    ) AS CashOutflow,

                    ISNULL(
                        (
                            SELECT SUM(DueAmount)
                            FROM Sales
                            WHERE
                                TenantId = @TenantId
                                AND IsDeleted = 0
                                AND DueAmount > 0
                        ),
                        0
                    ) AS TotalReceivables,

                    ISNULL(
                        (
                            SELECT SUM(DueAmount)
                            FROM Purchases
                            WHERE
                                TenantId = @TenantId
                                AND IsDeleted = 0
                                AND DueAmount > 0
                        ),
                        0
                    ) AS TotalPayables;
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

            var response =
                await connection
                    .QueryFirstAsync<FinancialDashboardDto>(
                        command);

            response.NetCashFlow =
                response.CashInflow
                - response.CashOutflow;

            return ApiResponse<FinancialDashboardDto>
                .SuccessResponse(response);
        }
    }
}