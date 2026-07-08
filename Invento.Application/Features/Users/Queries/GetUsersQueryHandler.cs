using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Users.DTOs;
using Invento.Application.Interfaces;
using Invento.Shared.Pagination;
using System.Data;

namespace Invento.Application.Features.Users.Queries
{
    public class GetUsersQueryHandler
        : IQueryHandler<
            GetUsersQuery,
            ApiResponse<PagedResponse<UserDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetUsersQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<PagedResponse<UserDto>>> Handle(
            GetUsersQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await ((System.Data.Common.DbConnection)connection)
                    .OpenAsync(cancellationToken);
            }

            var sql = @"
            SELECT
                Id,
                TenantId,
                FullName,
                Email,
                Role,
                IsActive,
                MustChangePassword,
                CreatedAt,
                CreatedBy,
                CreatedByUserId
            FROM Users
            WHERE
                TenantId = @TenantId
                AND IsDeleted = 0
                AND (
                    @SearchTerm IS NULL
                    OR FullName LIKE '%' + @SearchTerm + '%'
                    OR Email LIKE '%' + @SearchTerm + '%'
                )
            ORDER BY FullName
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;

            SELECT COUNT(*)
            FROM Users
            WHERE
                TenantId = @TenantId
                AND IsDeleted = 0
                AND (
                    @SearchTerm IS NULL
                    OR FullName LIKE '%' + @SearchTerm + '%'
                    OR Email LIKE '%' + @SearchTerm + '%'
                );
            ";

            var parameters = new
            {
                TenantId = _currentTenant.TenantId,

                SearchTerm = string.IsNullOrWhiteSpace(request.SearchTerm)
                    ? null
                    : request.SearchTerm.Trim(),

                Offset =
                    (request.PageNumber - 1) *
                    request.PageSize,

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

            var users =
                (await multi.ReadAsync<UserDto>())
                .ToList();

            var totalCount =
                await multi.ReadSingleAsync<int>();

            var response =
                new PagedResponse<UserDto>
                {
                    Items = users,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalCount
                };

            return ApiResponse<PagedResponse<UserDto>>
                .SuccessResponse(response);
        }
    }
}