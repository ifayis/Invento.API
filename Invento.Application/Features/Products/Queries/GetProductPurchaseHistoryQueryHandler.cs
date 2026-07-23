using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Products.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Products.Queries
{
    public class GetProductPurchaseHistoryQueryHandler
        : IQueryHandler<
            GetProductPurchaseHistoryQuery,
            ApiResponse<List<ProductPurchaseHistoryDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetProductPurchaseHistoryQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<ProductPurchaseHistoryDto>>> Handle(
            GetProductPurchaseHistoryQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            const string sql = @"
            SELECT
                p.Id AS PurchaseId,
                p.PurchaseNumber,
                p.PurchaseDate,
                pi.Quantity,
                pi.UnitCost,
                pi.TotalPrice,
                s.Name AS SupplierName
            FROM PurchaseItems pi
            INNER JOIN Purchases p
                ON pi.PurchaseId = p.Id
            INNER JOIN Suppliers s
                ON p.SupplierId = s.Id
            WHERE
                pi.ProductId = @ProductId
                AND pi.TenantId = @TenantId
                AND p.IsDeleted = 0
            ORDER BY p.PurchaseDate DESC;
            ";

            var result =
                (await connection.QueryAsync<ProductPurchaseHistoryDto>(
                    sql,
                    new
                    {
                        request.ProductId,
                        TenantId = _currentTenant.TenantId
                    }))
                .ToList();

            return ApiResponse<List<ProductPurchaseHistoryDto>>
                .SuccessResponse(result);
        }
    }
}