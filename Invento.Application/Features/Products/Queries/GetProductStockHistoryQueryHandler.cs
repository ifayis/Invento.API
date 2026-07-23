using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Products.DTOs;
using Invento.Application.Interfaces;
using Invento.Application.Common.Interface;

namespace Invento.Application.Features.Products.Queries
{
    public class GetProductStockHistoryQueryHandler
        : IQueryHandler<
            GetProductStockHistoryQuery,
            ApiResponse<List<ProductStockHistoryDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetProductStockHistoryQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<ProductStockHistoryDto>>> Handle(
            GetProductStockHistoryQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            const string sql = @"
            SELECT
                sm.Id AS StockMovementId,
                sm.CreatedAt AS MovementDate,
                sm.MovementType,
                sm.Quantity,
                sm.CurrentStockAfterMovement,
                sm.ReferenceNumber,
                sm.Remarks
            FROM StockMovements sm
            WHERE
                sm.ProductId = @ProductId
                AND sm.TenantId = @TenantId
                AND sm.IsDeleted = 0
            ORDER BY sm.CreatedAt DESC;
            ";

            var history =
                (await connection.QueryAsync<ProductStockHistoryDto>(
                    sql,
                    new
                    {
                        request.ProductId,
                        TenantId = _currentTenant.TenantId
                    }))
                .ToList();

            return ApiResponse<List<ProductStockHistoryDto>>
                .SuccessResponse(history);
        }
    }
}