using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Balance.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Enums;

namespace Invento.Application.Features.Balance.Queries
{
    public class GetCashFlowQueryHandler
        : IQueryHandler<
            GetCashFlowQuery,
            ApiResponse<CashFlowDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetCashFlowQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<CashFlowDto>> Handle(
            GetCashFlowQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            const string sql = @"
            SELECT
                TransactionDate AS Date,
                TransactionType,
                CONCAT('TXN-', CONVERT(varchar(36), Id)) AS ReferenceNumber,

                CASE
                    WHEN TransactionType IN (@Sale, @ManualIncome)
                        THEN Amount
                    ELSE 0
                END AS Income,

                CASE
                    WHEN TransactionType IN (@Purchase, @ManualExpense)
                        THEN Amount
                    ELSE 0
                END AS Expense

            FROM CashTransactions
            WHERE
                TenantId = @TenantId
                AND IsDeleted = 0
                AND TransactionDate >= @FromDate
                AND TransactionDate <= @ToDate
            ORDER BY TransactionDate;
            ";

            var transactions =
                (await connection.QueryAsync<CashFlowTransactionDto>(
                    sql,
                new
                {
                    TenantId = _currentTenant.TenantId,
                    request.FromDate,
                    request.ToDate,

                    Sale = CashTransactionType.Sale,
                    Purchase = CashTransactionType.Purchase,
                    ManualIncome = CashTransactionType.ManualIncome,
                    ManualExpense = CashTransactionType.ManualExpense
                }))
                .ToList();

            var response =
                new CashFlowDto
                {
                    Transactions = transactions,
                    TotalIncome =
                        transactions.Sum(x => x.Income),

                    TotalExpense =
                        transactions.Sum(x => x.Expense)
                };

            response.NetCashFlow =
                response.TotalIncome -
                response.TotalExpense;

            return ApiResponse<CashFlowDto>
                .SuccessResponse(response);
        }
    }
}