using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Products.DTOs;
using Invento.Application.Common.Interface;
namespace Invento.Application.Features.Products.Queries
{
    public class GetProductsQueryHandler
        : IQueryHandler<
            GetProductsQuery,
            ApiResponse<PagedResponse<ProductDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public GetProductsQueryHandler(
            IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<
            ApiResponse<PagedResponse<ProductDto>>> Handle(
            GetProductsQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
            SELECT
                p.Id,
                p.Name,
                p.SKU,
                p.CostPrice,
                p.SellingPrice,
                p.CurrentStock,
                c.Name AS CategoryName,
                p.CreatedAt
            FROM Products p
            INNER JOIN Categories c
                ON p.CategoryId = c.Id
            WHERE p.IsDeleted = 0
            AND
                (@Search IS NULL
                OR p.Name LIKE '%' + @Search + '%'
                OR p.SKU LIKE '%' + @Search + '%')
            ORDER BY p.CreatedAt DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;

            SELECT COUNT(*)
            FROM Products p
            WHERE p.IsDeleted = 0
            AND
                (@Search IS NULL
                OR p.Name LIKE '%' + @Search + '%'
                OR p.SKU LIKE '%' + @Search + '%');
            ";

            var parameters = new
            {
                Search = request.Search,
                Offset =
                    (request.PageNumber - 1)
                    * request.PageSize,
                request.PageSize
            };

            using var multi = await connection
                .QueryMultipleAsync(sql, parameters);

            var products = await multi
                .ReadAsync<ProductDto>();

            var totalRecords =
                await multi.ReadFirstAsync<int>();

            var response = new PagedResponse<ProductDto>
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