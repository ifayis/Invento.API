using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Reports.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetSupplierPurchaseReportQueryHandler
        : IQueryHandler<
            GetSupplierPurchaseReportQuery,
            ApiResponse<List<SupplierPurchaseReportDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetSupplierPurchaseReportQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<SupplierPurchaseReportDto>>> Handle(
            GetSupplierPurchaseReportQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
            SELECT
                s.Id AS SupplierId,
                s.Name AS SupplierName,
                COUNT(p.Id) AS TotalOrders,
                ISNULL(SUM(p.TotalAmount),0) AS TotalPurchases
            FROM Suppliers s
            INNER JOIN Purchases p
                ON s.Id = p.SupplierId
            WHERE
                p.TenantId = @TenantId
                AND p.IsDeleted = 0
                AND
                (
                    @FromDate IS NULL
                    OR p.PurchaseDate >= @FromDate
                )
                AND
                (
                    @ToDate IS NULL
                    OR p.PurchaseDate <= @ToDate
                )
            GROUP BY
                s.Id,
                s.Name
            ORDER BY
                TotalPurchases DESC;
            ";

            var result =
                (await connection.QueryAsync<SupplierPurchaseReportDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId,
                        request.FromDate,
                        request.ToDate
                    }))
                .ToList();

            return ApiResponse<List<SupplierPurchaseReportDto>>
                .SuccessResponse(result);
        }
    }
}