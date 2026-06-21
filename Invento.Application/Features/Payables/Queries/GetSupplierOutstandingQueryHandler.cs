using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Payables.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Payables.Queries
{
    public class GetSupplierOutstandingQueryHandler
        : IQueryHandler<
            GetSupplierOutstandingQuery,
            ApiResponse<List<SupplierOutstandingDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetSupplierOutstandingQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<SupplierOutstandingDto>>> Handle(
            GetSupplierOutstandingQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
                SELECT
                    s.Id AS SupplierId,
                    s.Name AS SupplierName,
                    SUM(p.TotalAmount) AS TotalPurchases,
                    SUM(p.PaidAmount) AS TotalPaid,
                    SUM(p.DueAmount) AS OutstandingAmount
                FROM Suppliers s
                INNER JOIN Purchases p
                    ON s.Id = p.SupplierId
                WHERE
                    s.TenantId = @TenantId
                    AND p.TenantId = @TenantId
                    AND s.IsDeleted = 0
                    AND p.IsDeleted = 0
                    AND p.DueAmount > 0
                GROUP BY
                    s.Id,
                    s.Name
                ORDER BY
                    OutstandingAmount DESC";

            var result =
                (await connection.QueryAsync<SupplierOutstandingDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId
                    }))
                .ToList();

            return ApiResponse<List<SupplierOutstandingDto>>
                .SuccessResponse(result);
        }
    }
}