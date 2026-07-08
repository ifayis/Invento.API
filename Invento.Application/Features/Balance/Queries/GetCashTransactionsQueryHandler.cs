using System.Data;
using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Balance.DTOs;
using Invento.Application.Interfaces;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.Balance.Queries
{
    public class GetCashTransactionsQueryHandler
        : IQueryHandler<
            GetCashTransactionsQuery,
            ApiResponse<PagedResponse<CashTransactionDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetCashTransactionsQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<
            ApiResponse<PagedResponse<CashTransactionDto>>> Handle(
            GetCashTransactionsQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await ((System.Data.Common.DbConnection)connection)
                    .OpenAsync(cancellationToken);
            }

            const string sql = """
                SELECT
                    Id,
                    TransactionType,
                    Amount,
                    Description,
                    TransactionDate
                FROM CashTransactions
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                ORDER BY
                    TransactionDate DESC,
                    Id DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT_BIG(*)
                FROM CashTransactions
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0;
                """;

            var parameters = new
            {
                TenantId = _currentTenant.TenantId,
                Offset = checked(
                    (request.PageNumber - 1)
                    * request.PageSize),
                request.PageSize
            };

            var command =
                new CommandDefinition(
                    commandText: sql,
                    parameters: parameters,
                    commandTimeout: 30,
                    cancellationToken: cancellationToken);

            using var multi =
                await connection.QueryMultipleAsync(command);

            var items =
                (await multi.ReadAsync<CashTransactionDto>())
                .ToList();

            var totalRecords =
                await multi.ReadSingleAsync<long>();

            return ApiResponse<
                PagedResponse<CashTransactionDto>>
                .SuccessResponse(
                    new PagedResponse<CashTransactionDto>
                    {
                        Items = items,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalCount =
                            checked((int)totalRecords)
                    });
        }
    }
}