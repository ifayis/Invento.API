using System.Data;
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

        public async Task<
            ApiResponse<PagedResponse<CustomerDto>>> Handle(
            GetCustomersQuery request,
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

                SELECT COUNT_BIG(*)
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
                    sql,
                    parameters,
                    commandTimeout: 30,
                    cancellationToken: cancellationToken);

            using var multi =
                await connection.QueryMultipleAsync(command);

            var customers =
                (await multi.ReadAsync<CustomerDto>())
                .ToList();

            var totalRecords =
                await multi.ReadSingleAsync<long>();

            return ApiResponse<PagedResponse<CustomerDto>>
                .SuccessResponse(
                    new PagedResponse<CustomerDto>
                    {
                        Items = customers,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalCount =
                            checked((int)totalRecords)
                    });
        }
    }
}