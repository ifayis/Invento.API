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

            var sql = @"
            SELECT

            ISNULL(
            (
                SELECT SUM(TotalAmount)
                FROM Sales
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
            ),0) AS SalesIncome,

            ISNULL(
            (
                SELECT SUM(TotalAmount)
                FROM Purchases
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
            ),0) AS PurchaseExpense,

            ISNULL(
            (
                SELECT SUM(Amount)
                FROM CashTransactions
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                    AND TransactionType = @ManualIncome
            ),0) AS ManualIncome,

            ISNULL(
            (
                SELECT SUM(Amount)
                FROM CashTransactions
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                    AND TransactionType = @ManualExpense
            ),0) AS ManualExpense
            ";

            var dashboard =
                await connection.QueryFirstAsync
                <BalanceDashboardDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId,

                        ManualIncome =
                            (int)CashTransactionType.ManualIncome,

                        ManualExpense =
                            (int)CashTransactionType.ManualExpense
                    });

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