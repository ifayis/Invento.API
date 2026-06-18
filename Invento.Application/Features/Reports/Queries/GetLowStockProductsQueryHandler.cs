using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Reports.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetLowStockProductsQueryHandler
    : IQueryHandler<
    GetLowStockProductsQuery,
    ApiResponse<List<LowStockProductDto>>>
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

        public async Task<ApiResponse<List<LowStockProductDto>>> Handle(
            GetLowStockProductsQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
        SELECT
            Id AS ProductId,
            Name AS ProductName,
            SKU,
            CurrentStock,
            LowStockThreshold
        FROM Products
        WHERE
            TenantId = @TenantId
            AND IsDeleted = 0
            AND CurrentStock <= LowStockThreshold
        ORDER BY CurrentStock ASC;
        ";

            var products =
                (await connection.QueryAsync<LowStockProductDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId
                    }))
                .ToList();

            return ApiResponse<List<LowStockProductDto>>
                .SuccessResponse(products);
        }
    }

}
