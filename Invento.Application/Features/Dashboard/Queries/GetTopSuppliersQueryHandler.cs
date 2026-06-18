using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Dashboard.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Dashboard.Queries
{
    public class GetTopSuppliersQueryHandler
        : IQueryHandler<
            GetTopSuppliersQuery,
            ApiResponse<List<TopSupplierDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetTopSuppliersQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<TopSupplierDto>>> Handle(
            GetTopSuppliersQuery request,
            CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT TOP (@Count)
                    s.Id AS SupplierId,
                    s.Name AS SupplierName,
                    COUNT(p.Id) AS TotalPurchases,
                    SUM(p.TotalAmount) AS TotalAmount
                FROM Purchases p
                INNER JOIN Suppliers s
                    ON p.SupplierId = s.Id
                WHERE p.TenantId = @TenantId
                GROUP BY s.Id, s.Name
                ORDER BY TotalAmount DESC";

            var result = (await connection.QueryAsync<TopSupplierDto>(
                sql,
                new
                {
                    TenantId = _currentTenant.TenantId,
                    request.Count
                }))
                .ToList();

            return ApiResponse<List<TopSupplierDto>>
                .SuccessResponse(result);
        }
    }
}