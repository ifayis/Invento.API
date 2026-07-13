using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Balance.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Enums;

namespace Invento.Application.Features.Balance.Queries
{
    public class GetBalanceDashboardQueryHandler
        : IQueryHandler<
            GetBalanceDashboardQuery,
            ApiResponse<BalanceDashboardDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetBalanceDashboardQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<BalanceDashboardDto>> Handle(
            GetBalanceDashboardQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            const string sql = """
                SELECT
                    ISNULL(
                        SUM(
                            CASE
                                WHEN TransactionType = @Sale
                                THEN Amount
                                ELSE 0
                            END
                        ),
                        0
                    ) AS SalesIncome,

                    ISNULL(
                        SUM(
                            CASE
                                WHEN TransactionType = @Purchase
                                THEN Amount
                                ELSE 0
                            END
                        ),
                        0
                    ) AS PurchaseExpense,

                    ISNULL(
                        SUM(
                            CASE
                                WHEN TransactionType = @ManualIncome
                                THEN Amount
                                ELSE 0
                            END
                        ),
                        0
                    ) AS ManualIncome,

                    ISNULL(
                        SUM(
                            CASE
                                WHEN TransactionType = @ManualExpense
                                THEN Amount
                                ELSE 0
                            END
                        ),
                        0
                    ) AS ManualExpense
                FROM CashTransactions
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0;
                """;

            Console.WriteLine("balance dashboard Handler Executed");

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

            var dashboard =
                await connection
                    .QueryFirstAsync<BalanceDashboardDto>(
                        command);

            dashboard.TotalIncome =
                dashboard.SalesIncome
                + dashboard.ManualIncome;

            dashboard.TotalExpense =
                dashboard.PurchaseExpense
                + dashboard.ManualExpense;

            dashboard.CurrentBalance =
                dashboard.TotalIncome
                - dashboard.TotalExpense;

            return ApiResponse<BalanceDashboardDto>
                .SuccessResponse(dashboard);
        }
    }
}