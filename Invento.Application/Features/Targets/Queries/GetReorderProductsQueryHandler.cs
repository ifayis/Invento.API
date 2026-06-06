using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Targets.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Targets.Queries
{
    public class GetReorderProductsQueryHandler
        : IQueryHandler<
            GetReorderProductsQuery,
            ApiResponse<List<ReorderProductDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetReorderProductsQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<ReorderProductDto>>> Handle(
            GetReorderProductsQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
            SELECT
                p.Id AS ProductId,
                p.Name AS ProductName,
                c.Name AS CategoryName,
                p.CurrentStock,
                p.LowStockThreshold,

                CASE
                    WHEN p.CurrentStock <= 0
                        THEN p.LowStockThreshold * 2

                    ELSE
                        p.LowStockThreshold - p.CurrentStock
                END
                AS RecommendedOrderQuantity

            FROM Products p

            INNER JOIN Categories c
                ON p.CategoryId = c.Id

            WHERE
                p.TenantId = @TenantId
                AND p.IsDeleted = 0
                AND p.CurrentStock <= p.LowStockThreshold

            ORDER BY
                p.CurrentStock ASC
            ";

            var result =
                await connection.QueryAsync<ReorderProductDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId
                    });

            return ApiResponse<List<ReorderProductDto>>
                .SuccessResponse(result.ToList());
        }
    }
}