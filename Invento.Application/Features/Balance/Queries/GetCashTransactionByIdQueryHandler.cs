using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Balance.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Balance.Queries
{
    public class GetCashTransactionByIdQueryHandler
        : IQueryHandler<
            GetCashTransactionByIdQuery,
            ApiResponse<CashTransactionDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetCashTransactionByIdQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<CashTransactionDto>> Handle(
            GetCashTransactionByIdQuery request,
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
                Id = @Id
                AND TenantId = @TenantId
                AND IsDeleted = 0
            ";

            var transaction =
                await connection.QueryFirstOrDefaultAsync
                <CashTransactionDto>(
                    sql,
                    new
                    {
                        request.Id,
                        TenantId = _currentTenant.TenantId
                    });

            if (transaction is null)
            {
                return ApiResponse<CashTransactionDto>
                    .FailureResponse(
                        new()
                        {
                            "Transaction not found"
                        });
            }

            return ApiResponse<CashTransactionDto>
                .SuccessResponse(transaction);
        }
    }
}