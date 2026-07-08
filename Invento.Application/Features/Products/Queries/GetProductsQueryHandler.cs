using System.Data;
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

        public async Task<
            ApiResponse<PagedResponse<ProductDto>>> Handle(
            GetProductsQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await ((System.Data.Common.DbConnection)connection)
                    .OpenAsync(cancellationToken);
            }

            const string sql = """
                SELECT
                    p.Id,
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
                    AND c.TenantId = p.TenantId
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

                SELECT COUNT_BIG(*)
                FROM Products p
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

            var search =
                string.IsNullOrWhiteSpace(request.Search)
                    ? null
                    : request.Search.Trim();

            var parameters = new
            {
                TenantId = _currentTenant.TenantId,
                Search = search,
                Offset =
                    checked(
                        (request.PageNumber - 1)
                        * request.PageSize),
                request.PageSize
            };

            var command =
                new CommandDefinition(
                    commandText: sql,
                    parameters: parameters,
                    commandTimeout: 30,
                    cancellationToken: cancellationToken);

            using var multi =
                await connection.QueryMultipleAsync(command);

            var products =
                (await multi.ReadAsync<ProductDto>())
                .ToList();

            var totalRecords =
                await multi.ReadSingleAsync<long>();

            var response =
                new PagedResponse<ProductDto>
                {
                    Items = products,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = checked((int)totalRecords)
                };

            return ApiResponse<
                PagedResponse<ProductDto>>
                .SuccessResponse(response);
        }
    }
}