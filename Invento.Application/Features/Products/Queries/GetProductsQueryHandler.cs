using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Products.DTOs;
using Invento.Application.Interfaces;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.Products.Queries
{
    public class GetProductsQueryHandler
        : IQueryHandler<
            GetProductsQuery,
            ApiResponse<PagedResponse<ProductDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetProductsQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<PagedResponse<ProductDto>>> Handle(
            GetProductsQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var search =
                string.IsNullOrWhiteSpace(request.Search)
                    ? null
                    : request.Search.Trim();

            const string sql = """
                SELECT
                    p.Id,
                    p.CategoryId,
                    p.Name,
                    p.SKU,
                    p.CostPrice,
                    p.SellingPrice,
                    p.CurrentStock,
                    p.IsDeleted,
                    c.Name AS CategoryName,
                    p.CreatedAt,
                    p.LowStockThreshold,
                    p.CriticalStockThreshold
                FROM Products p
                INNER JOIN Categories c
                    ON c.Id = p.CategoryId
                    AND c.TenantId = @TenantId
                    AND c.IsDeleted = 0
                WHERE
                    p.TenantId = @TenantId
                    AND p.IsDeleted = 0
                    AND
                    (
                        @Search IS NULL
                        OR p.Name LIKE '%' + @Search + '%'
                        OR p.SKU LIKE '%' + @Search + '%'
                    )
                ORDER BY
                    p.CreatedAt DESC,
                    p.Id DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(*)
                FROM Products p
                INNER JOIN Categories c
                    ON c.Id = p.CategoryId
                    AND c.TenantId = @TenantId
                    AND c.IsDeleted = 0
                WHERE
                    p.TenantId = @TenantId
                    AND p.IsDeleted = 0
                    AND
                    (
                        @Search IS NULL
                        OR p.Name LIKE '%' + @Search + '%'
                        OR p.SKU LIKE '%' + @Search + '%'
                    );
                """;

            var parameters =
                new
                {
                    TenantId = _currentTenant.TenantId,
                    Search = search,
                    Offset =
                        (request.PageNumber - 1)
                        * request.PageSize,
                    request.PageSize
                };

            var command =
                new CommandDefinition(
                    sql,
                    parameters,
                    cancellationToken:
                        cancellationToken);

            using var multi =
                await connection.QueryMultipleAsync(
                    command);

            var products =
                (await multi.ReadAsync<ProductDto>())
                    .ToList();

            var totalRecords =
                await multi.ReadSingleAsync<int>();

            var response =
                new PagedResponse<ProductDto>
                {
                    Items = products,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalRecords
                };

            return ApiResponse<
                PagedResponse<ProductDto>>
                .SuccessResponse(response);
        }
    }
}