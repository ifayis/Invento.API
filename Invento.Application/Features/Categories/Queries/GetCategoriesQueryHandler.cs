using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Categories.DTOs;
using Invento.Application.Interfaces;

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

            var sql = @"
        SELECT
            Id,
            Name,
            CreatedAt
        FROM Categories
        WHERE IsDeleted = 0
        AND TenantId = @TenantId
        AND
        (
            @Search IS NULL
            OR Name LIKE '%' + @Search + '%'
        )
        ORDER BY CreatedAt DESC
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(*)
        FROM Categories
        WHERE IsDeleted = 0
        AND TenantId = @TenantId
        AND
        (
            @Search IS NULL
            OR Name LIKE '%' + @Search + '%'
        );
        ";

            var parameters = new
            {
                TenantId = _currentTenant.TenantId,
                request.Search,
                Offset =
                    (request.PageNumber - 1)
                    * request.PageSize,
                request.PageSize
            };

            using var multi =
                await connection.QueryMultipleAsync(
                    sql,
                    parameters);

            var categories =
                await multi.ReadAsync<CategoryDto>();

            var totalRecords =
                await multi.ReadFirstAsync<int>();

            var response =
                new PagedResponse<CategoryDto>
                {
                    Items = categories,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalRecords = totalRecords
                };

            return ApiResponse<
                PagedResponse<CategoryDto>>
                .SuccessResponse(response);
        }
    }
}