using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.StockMovements.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.StockMovements.Queries
{
    public class GetFastMovingProductsQueryHandler
        : IQueryHandler<
            GetFastMovingProductsQuery,
            ApiResponse<List<FastMovingProductDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetFastMovingProductsQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<FastMovingProductDto>>> Handle(
            GetFastMovingProductsQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
            SELECT TOP (@Top)

                p.Id AS ProductId,

                p.Name AS ProductName,

                c.Name AS CategoryName,

                SUM(si.Quantity)
                    AS TotalQuantitySold,

                SUM(si.TotalPrice)
                    AS TotalRevenue,

                SUM(si.ProfitAmount)
                    AS TotalProfit,

                p.CurrentStock

            FROM SaleItems si

            INNER JOIN Sales s
                ON si.SaleId = s.Id

            INNER JOIN Products p
                ON si.ProductId = p.Id

            INNER JOIN Categories c
                ON p.CategoryId = c.Id

            WHERE
                s.IsDeleted = 0
                AND p.IsDeleted = 0
                AND s.TenantId = @TenantId
                AND si.TenantId = @TenantId

            GROUP BY
                p.Id,
                p.Name,
                c.Name,
                p.CurrentStock

            ORDER BY
                SUM(si.Quantity) DESC
            ";

            var result =
                await connection.QueryAsync<
                    FastMovingProductDto>(
                    sql,
                    new
                    {
                        Top = request.Top,
                        TenantId =
                            _currentTenant.TenantId
                    });

            return ApiResponse<
                List<FastMovingProductDto>>
                .SuccessResponse(
                    result.ToList()
                );
        }
    }
}