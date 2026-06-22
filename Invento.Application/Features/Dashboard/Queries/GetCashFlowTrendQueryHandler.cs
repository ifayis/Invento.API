using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Dashboard.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Dashboard.Queries
{
    public class GetCashFlowTrendQueryHandler
        : IQueryHandler<
            GetCashFlowTrendQuery,
            ApiResponse<List<CashFlowTrendDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetCashFlowTrendQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<CashFlowTrendDto>>> Handle(
            GetCashFlowTrendQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
            SELECT
                FORMAT(
                    TransactionDate,
                    'MMM yyyy'
                ) AS Month,

                SUM(
                    CASE
                        WHEN TransactionType IN (1,3)
                        THEN Amount
                        ELSE 0
                    END
                ) AS CashIn,

                SUM(
                    CASE
                        WHEN TransactionType IN (2,4)
                        THEN Amount
                        ELSE 0
                    END
                ) AS CashOut,

                SUM(
                    CASE
                        WHEN TransactionType IN (1,3)
                        THEN Amount
                        ELSE -Amount
                    END
                ) AS NetCashFlow

            FROM CashTransactions

            WHERE
                TenantId = @TenantId
                AND IsDeleted = 0

            GROUP BY
                YEAR(TransactionDate),
                MONTH(TransactionDate),
                FORMAT(TransactionDate,'MMM yyyy')

            ORDER BY
                YEAR(TransactionDate),
                MONTH(TransactionDate);
            ";

            var result =
                (await connection.QueryAsync<CashFlowTrendDto>(
                    sql,
                    new
                    {
                        TenantId =
                            _currentTenant.TenantId
                    }))
                .ToList();

            return ApiResponse<List<CashFlowTrendDto>>
                .SuccessResponse(result);
        }
    }
}