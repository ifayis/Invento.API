using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Targets.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Targets.Queries
{
    public class GetLowStockProductsQueryHandler
        : IQueryHandler<GetLowStockProductsQuery, ApiResponse<List<StockAlertDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetLowStockProductsQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<StockAlertDto>>> Handle(
                GetLowStockProductsQuery request,
                CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
            SELECT
                p.Id AS ProductId,
                p.Name AS ProductName,
                p.CategoryId,
                c.Name AS CategoryName,
                p.CurrentStock,
                p.LowStockThreshold,
                ts.CriticalStockThreshold,
                p.IsDeleted,
                'LOW' AS Status

            FROM Products p

            INNER JOIN Categories c
                ON p.CategoryId = c.Id

            INNER JOIN TenantSettings ts
                ON p.TenantId = ts.TenantId

            WHERE
                p.TenantId = @TenantId
                AND p.IsDeleted = 0

                AND p.CurrentStock <= p.LowStockThreshold

                AND p.CurrentStock > ts.CriticalStockThreshold

            ORDER BY p.CurrentStock ASC
            ";

            var result = await connection.QueryAsync <StockAlertDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId
                    }
            );

            return ApiResponse<List<StockAlertDto>>
                .SuccessResponse(result.ToList());
        }
    }
}