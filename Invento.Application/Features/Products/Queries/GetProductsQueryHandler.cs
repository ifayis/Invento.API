using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Products.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Products.Queries
{
    public class GetProductsQueryHandler
        : IQueryHandler<GetProductsQuery, ApiResponse<PagedResponse<ProductDto>>>
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
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
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
                ON p.CategoryId = c.Id
            WHERE
                p.TenantId = @TenantId
                AND c.TenantId = @TenantId
                AND
                (
                    @Search IS NULL
                    OR p.Name LIKE '%' + @Search + '%'
                    OR p.SKU LIKE '%' + @Search + '%'
                )
            ORDER BY p.CreatedAt DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;

            SELECT COUNT(*)
            FROM Products p
            WHERE
                p.TenantId = @TenantId
                AND
                (
                    @Search IS NULL
                    OR p.Name LIKE '%' + @Search + '%'
                    OR p.SKU LIKE '%' + @Search + '%'
                );
            ";

            var parameters = new
            {
                TenantId = _currentTenant.TenantId,
                Search = request.Search,
                Offset =
                    (request.PageNumber - 1)
                    * request.PageSize,
                request.PageSize
            };

            using var multi = await connection.QueryMultipleAsync(
                    sql,
                    parameters
                    
            );

            var products = (await multi.ReadAsync<ProductDto>()).ToList();

            var totalRecords = await multi.ReadFirstAsync<int>();

            var response =
                new PagedResponse<ProductDto>
                {
                    Items = products,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalRecords = totalRecords
                };

            return ApiResponse<
                PagedResponse<ProductDto>>
                .SuccessResponse(response);
        }
    }
}