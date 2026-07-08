using System.Data;
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

        public async Task<
            ApiResponse<PagedResponse<CategoryDto>>> Handle(
            GetCategoriesQuery request,
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
                    Id,
                    Name,
                    CreatedAt
                FROM Categories
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                    AND
                    (
                        @Search IS NULL
                        OR Name LIKE '%' + @Search + '%'
                    )
                ORDER BY
                    CreatedAt DESC,
                    Id DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT_BIG(*)
                FROM Categories
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                    AND
                    (
                        @Search IS NULL
                        OR Name LIKE '%' + @Search + '%'
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
                Offset = checked(
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

            var categories =
                (await multi.ReadAsync<CategoryDto>())
                .ToList();

            var totalRecords =
                await multi.ReadSingleAsync<long>();

            return ApiResponse<PagedResponse<CategoryDto>>
                .SuccessResponse(
                    new PagedResponse<CategoryDto>
                    {
                        Items = categories,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalCount =
                            checked((int)totalRecords)
                    });
        }
    }
}