using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Dashboard.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Dashboard.Queries
{
    public class GetTopProductsQueryHandler
        : IQueryHandler<
            GetTopProductsQuery,
            ApiResponse<List<TopProductDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetTopProductsQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<TopProductDto>>> Handle(
            GetTopProductsQuery request,
            CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT TOP (@Count)
                    p.Id AS ProductId,
                    p.Name AS ProductName,
                    SUM(si.Quantity) AS QuantitySold,
                    SUM(si.TotalPrice) AS Revenue
                FROM SaleItems si
                INNER JOIN Products p
                    ON si.ProductId = p.Id
                WHERE si.TenantId = @TenantId
                GROUP BY p.Id, p.Name
                ORDER BY QuantitySold DESC";

            var result = (await connection.QueryAsync<TopProductDto>(
                sql,
                new
                {
                    TenantId = _currentTenant.TenantId,
                    request.Count
                }))
                .ToList();

            return ApiResponse<List<TopProductDto>>
                .SuccessResponse(result);
        }
    }
}