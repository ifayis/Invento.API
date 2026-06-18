using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Dashboard.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Dashboard.Queries
{
    public class GetTopCustomersQueryHandler
        : IQueryHandler<
            GetTopCustomersQuery,
            ApiResponse<List<TopCustomerDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetTopCustomersQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<TopCustomerDto>>> Handle(
            GetTopCustomersQuery request,
            CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT TOP (@Count)
                    c.Id AS CustomerId,
                    c.Name AS CustomerName,
                    COUNT(s.Id) AS TotalOrders,
                    SUM(s.TotalAmount) AS TotalSpent
                FROM Sales s
                INNER JOIN Customers c
                    ON s.CustomerId = c.Id
                WHERE
                    s.TenantId = @TenantId
                    AND s.CustomerId IS NOT NULL
                GROUP BY c.Id, c.Name
                ORDER BY TotalSpent DESC";

            var result = (await connection.QueryAsync<TopCustomerDto>(
                sql,
                new
                {
                    TenantId = _currentTenant.TenantId,
                    request.Count
                }))
                .ToList();

            return ApiResponse<List<TopCustomerDto>>
                .SuccessResponse(result);
        }
    }
}