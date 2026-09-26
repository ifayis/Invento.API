using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Categories.DTOs;
using Invento.Application.Interfaces;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.Categories.Queries
{
    public class GetCategoriesQueryHandler
        : IQueryHandler<
            GetCategoriesQuery,
            ApiResponse<PagedResponse<CategoryDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetCategoriesQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<PagedResponse<CategoryDto>>> Handle(
            GetCategoriesQuery request,
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
                c.Id,
                c.Name,
                c.CreatedAt,
                COUNT(p.Id) AS ProductCount
            FROM Categories c
            LEFT JOIN Products p
                ON p.CategoryId = c.Id
                AND p.TenantId = @TenantId
                AND p.IsDeleted = 0
            WHERE
                c.TenantId = @TenantId
                AND c.IsDeleted = 0
                AND
                (
                    @Search IS NULL
                    OR c.Name LIKE '%' + @Search + '%'
                )
            GROUP BY
                c.Id,
                c.Name,
                c.CreatedAt
            ORDER BY
                c.CreatedAt DESC,
                c.Id DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;

            SELECT COUNT(*)
            FROM Categories
            WHERE
                TenantId = @TenantId
                AND
                (
                    @IncludeDeleted = 1
                    OR IsDeleted = 0
                )
                AND
                (
                    @Search IS NULL
                    OR Name LIKE '%' + @Search + '%'
                );
            """;

            var parameters = new
            {
                TenantId = _currentTenant.TenantId,
                Search = search,
                IncludeDeleted = request.IncludeDeleted,
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

            var categories =
                (await multi.ReadAsync<CategoryDto>())
                .ToList();

            var totalRecords =
                await multi.ReadSingleAsync<int>();

            var response =
                new PagedResponse<CategoryDto>
                {
                    Items = categories,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalRecords
                };

            return ApiResponse<
                PagedResponse<CategoryDto>>
                .SuccessResponse(response);
        }
    }
}