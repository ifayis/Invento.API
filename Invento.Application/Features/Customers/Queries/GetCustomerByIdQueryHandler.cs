using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Customer.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Customers.Queries;

public class GetCustomerByIdQueryHandler
    : IQueryHandler<
        GetCustomerByIdQuery,
        ApiResponse<CustomerDto>>
{
    private readonly IDbConnectionFactory
        _connectionFactory;

    private readonly ICurrentTenantService
        _currentTenant;

    public GetCustomerByIdQueryHandler(
        IDbConnectionFactory connectionFactory,
        ICurrentTenantService currentTenant)
    {
        _connectionFactory = connectionFactory;
        _currentTenant = currentTenant;
    }

    public async Task<ApiResponse<CustomerDto>>
        Handle(
            GetCustomerByIdQuery request,
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
    Id = @Id
    AND TenantId = @TenantId
    AND IsDeleted = 0
";

        var customer =
            await connection
            .QueryFirstOrDefaultAsync<CustomerDto>(
                sql,
                new
                {
                    request.Id,

                    TenantId =
                        _currentTenant.TenantId
                });

        if (customer is null)
        {
            return ApiResponse<CustomerDto>
                .FailureResponse(
                    new List<string>
                    {
                        "Customer not found"
                    });
        }

        return ApiResponse<CustomerDto>
            .SuccessResponse(customer);
    }
}