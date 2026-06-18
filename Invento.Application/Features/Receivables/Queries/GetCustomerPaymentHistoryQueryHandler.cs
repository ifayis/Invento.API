using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Receivables.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Receivables.Queries
{
    public class GetCustomerPaymentHistoryQueryHandler
        : IQueryHandler<
            GetCustomerPaymentHistoryQuery,
            ApiResponse<List<CustomerPaymentHistoryDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetCustomerPaymentHistoryQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<CustomerPaymentHistoryDto>>> Handle(
            GetCustomerPaymentHistoryQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
                SELECT
                    cp.Id,
                    cp.SaleId,
                    s.InvoiceNumber,
                    cp.Amount,
                    cp.PaymentDate,
                    cp.Remarks
                FROM CustomerPayments cp
                INNER JOIN Sales s
                    ON cp.SaleId = s.Id
                WHERE
                    cp.CustomerId = @CustomerId
                    AND cp.TenantId = @TenantId
                    AND cp.IsDeleted = 0
                ORDER BY
                    cp.PaymentDate DESC";

            var result =
                (await connection.QueryAsync<CustomerPaymentHistoryDto>(
                    sql,
                    new
                    {
                        request.CustomerId,
                        TenantId = _currentTenant.TenantId
                    }))
                .ToList();

            return ApiResponse<List<CustomerPaymentHistoryDto>>
                .SuccessResponse(result);
        }
    }
}