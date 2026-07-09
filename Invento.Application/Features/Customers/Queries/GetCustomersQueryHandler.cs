using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Customers.DTOs;
using Invento.Application.Interfaces;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.Customers.Queries
{
    public class GetCustomersQueryHandler
        : IQueryHandler<
            GetCustomersQuery,
            ApiResponse<PagedResponse<CustomerDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetCustomersQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<PagedResponse<CustomerDto>>> Handle(
            GetCustomersQuery request,
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
                    Id,
                    Name,
                    Email,
                    PhoneNumber,
                    Address,
                    IsDeleted
                FROM Customers
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                    AND
                    (
                        @Search IS NULL
                        OR Name LIKE '%' + @Search + '%'
                        OR Email LIKE '%' + @Search + '%'
                        OR PhoneNumber LIKE '%' + @Search + '%'
                    )
                ORDER BY
                    Name ASC,
                    Id ASC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(*)
                FROM Customers
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                    AND
                    (
                        @Search IS NULL
                        OR Name LIKE '%' + @Search + '%'
                        OR Email LIKE '%' + @Search + '%'
                        OR PhoneNumber LIKE '%' + @Search + '%'
                    );
                """;

            var parameters = new
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

            var customers =
                (await multi.ReadAsync<CustomerDto>())
                .ToList();

            var totalRecords =
                await multi.ReadSingleAsync<int>();

            var response =
                new PagedResponse<CustomerDto>
                {
                    Items = customers,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalRecords
                };

            return ApiResponse<
                PagedResponse<CustomerDto>>
                .SuccessResponse(response);
        }
    }
}