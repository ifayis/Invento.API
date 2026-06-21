using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Payables.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Payables.Queries
{
    public class GetSupplierPaymentHistoryQueryHandler
        : IQueryHandler<
            GetSupplierPaymentHistoryQuery,
            ApiResponse<List<SupplierPaymentHistoryDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetSupplierPaymentHistoryQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<SupplierPaymentHistoryDto>>> Handle(
            GetSupplierPaymentHistoryQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
                SELECT
                    sp.Id,
                    sp.PurchaseId,
                    p.PurchaseNumber,
                    sp.Amount,
                    sp.PaymentDate,
                    sp.Remarks
                FROM SupplierPayments sp
                INNER JOIN Purchases p
                    ON sp.PurchaseId = p.Id
                WHERE
                    sp.SupplierId = @SupplierId
                    AND sp.TenantId = @TenantId
                    AND sp.IsDeleted = 0
                ORDER BY
                    sp.PaymentDate DESC";

            var result =
                (await connection.QueryAsync<SupplierPaymentHistoryDto>(
                    sql,
                    new
                    {
                        request.SupplierId,
                        TenantId = _currentTenant.TenantId
                    }))
                .ToList();

            return ApiResponse<List<SupplierPaymentHistoryDto>>
                .SuccessResponse(result);
        }
    }
}