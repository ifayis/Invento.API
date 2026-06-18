using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Receivables.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Receivables.Queries
{
    public class GetCustomerOutstandingQueryHandler
        : IQueryHandler<
            GetCustomerOutstandingQuery,
            ApiResponse<List<CustomerOutstandingDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetCustomerOutstandingQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<CustomerOutstandingDto>>> Handle(
            GetCustomerOutstandingQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
                SELECT
                    c.Id AS CustomerId,
                    c.Name AS CustomerName,
                    SUM(s.TotalAmount) AS TotalSales,
                    SUM(s.PaidAmount) AS TotalPaid,
                    SUM(s.DueAmount) AS OutstandingAmount
                FROM Customers c
                INNER JOIN Sales s
                    ON c.Id = s.CustomerId
                WHERE
                    c.TenantId = @TenantId
                    AND s.TenantId = @TenantId
                    AND c.IsDeleted = 0
                    AND s.IsDeleted = 0
                    AND s.DueAmount > 0
                GROUP BY
                    c.Id,
                    c.Name
                ORDER BY
                    OutstandingAmount DESC";

            var result =
                (await connection.QueryAsync<CustomerOutstandingDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId
                    }))
                .ToList();

            return ApiResponse<List<CustomerOutstandingDto>>
                .SuccessResponse(result);
        }
    }
}