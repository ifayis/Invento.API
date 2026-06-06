using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.StockMovements.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.StockMovements.Queries
{
    public class GetDeadStockProductsQueryHandler
        : IQueryHandler<
            GetDeadStockProductsQuery,
            ApiResponse<List<DeadStockProductDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetDeadStockProductsQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<DeadStockProductDto>>> Handle(
            GetDeadStockProductsQuery request,
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
                p.CostPrice,

                (p.CurrentStock * p.CostPrice)
                    AS InventoryValue,

                MAX(s.SaleDate)
                    AS LastSoldDate,

                DATEDIFF(
                    DAY,
                    MAX(s.SaleDate),
                    GETUTCDATE()
                ) AS DaysWithoutSale

            FROM Products p

            INNER JOIN Categories c
                ON p.CategoryId = c.Id

            LEFT JOIN SaleItems si
                ON p.Id = si.ProductId

            LEFT JOIN Sales s
                ON si.SaleId = s.Id
                AND s.IsDeleted = 0

            WHERE
                p.TenantId = @TenantId
                AND p.IsDeleted = 0

            GROUP BY
                p.Id,
                p.Name,
                c.Name,
                p.CurrentStock,
                p.CostPrice

            HAVING
                MAX(s.SaleDate) IS NULL

                OR

                MAX(s.SaleDate)
                    < DATEADD(
                        DAY,
                        -@Days,
                        GETUTCDATE()
                    )

            ORDER BY
                InventoryValue DESC
            ";

            var result =
                await connection.QueryAsync<
                    DeadStockProductDto>(
                    sql,
                    new
                    {
                        TenantId =
                            _currentTenant.TenantId,
                        request.Days
                    });

            return ApiResponse<
                List<DeadStockProductDto>>
                .SuccessResponse(
                    result.ToList()
                );
        }
    }
}