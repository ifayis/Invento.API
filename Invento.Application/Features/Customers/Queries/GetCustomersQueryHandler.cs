using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Customer.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Customers.Queries;

public class GetCustomersQueryHandler
    : IQueryHandler<
        GetCustomersQuery,
        ApiResponse<PagedResponse<CustomerDto>>>
{
    private readonly IDbConnectionFactory
        _connectionFactory;

    private readonly ICurrentTenantService
        _currentTenant;

    public GetCustomersQueryHandler(
        IDbConnectionFactory connectionFactory,
        ICurrentTenantService currentTenant)
    {
        _connectionFactory = connectionFactory;
        _currentTenant = currentTenant;
    }

    public async Task<
        ApiResponse<PagedResponse<CustomerDto>>>
        Handle(
            GetCustomersQuery request,
            CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var sql = @"
SELECT
    Id,
    Name,
    Email,
    PhoneNumber,
    Address,
    CreatedAt
FROM Customers
WHERE
    IsDeleted = 0
    AND TenantId = @TenantId
    AND
    (
        @Search IS NULL
        OR Name LIKE '%' + @Search + '%'
        OR Email LIKE '%' + @Search + '%'
        OR PhoneNumber LIKE '%' + @Search + '%'
    )

ORDER BY CreatedAt DESC

OFFSET @Offset ROWS
FETCH NEXT @PageSize ROWS ONLY;

SELECT COUNT(*)
FROM Customers
WHERE
    IsDeleted = 0
    AND TenantId = @TenantId
    AND
    (
        @Search IS NULL
        OR Name LIKE '%' + @Search + '%'
        OR Email LIKE '%' + @Search + '%'
        OR PhoneNumber LIKE '%' + @Search + '%'
    );
";

        var parameters = new
        {
            TenantId =
                _currentTenant.TenantId,

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

        var customers =
            await multi.ReadAsync<CustomerDto>();

        var totalRecords =
            await multi.ReadFirstAsync<int>();

        var response =
            new PagedResponse<CustomerDto>
            {
                Items = customers.ToList(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalRecords = totalRecords
            };

        return ApiResponse<
            PagedResponse<CustomerDto>>
            .SuccessResponse(response);
    }
}