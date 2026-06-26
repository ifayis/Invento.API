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

        public async Task<ApiResponse<PagedResponse<CashTransactionDto>>> Handle(
            GetCashTransactionsQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
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

            ORDER BY TransactionDate DESC

            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;

            SELECT COUNT(*)
            FROM CashTransactions
            WHERE
                TenantId = @TenantId
                AND IsDeleted = 0;
            ";

            var parameters = new
            {
                TenantId = _currentTenant.TenantId,

                Offset =
                    (request.PageNumber - 1)
                    * request.PageSize,

                request.PageSize
            };

            using var multi =
                await connection.QueryMultipleAsync(
                    sql,
                    parameters);

            var items =
                await multi.ReadAsync<CashTransactionDto>();

            var totalRecords =
                await multi.ReadFirstAsync<int>();

            var response =
                new PagedResponse<CashTransactionDto>
                {
                    Items = items.ToList(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalRecords
                };

            return ApiResponse<PagedResponse<CashTransactionDto>>
                .SuccessResponse(response);
        }
    }
}