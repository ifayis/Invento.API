using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Customers.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Customers.Queries
{
    public class GetInactiveCustomersQueryHandler
        : IQueryHandler<
            GetInactiveCustomersQuery,
            ApiResponse<List<InactiveCustomerDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetInactiveCustomersQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<InactiveCustomerDto>>> Handle(
            GetInactiveCustomersQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
            SELECT

                c.Id AS CustomerId,

                c.Name AS CustomerName,

                c.Email,

                c.PhoneNumber,

                MAX(s.SaleDate)
                    AS LastPurchaseDate,

                COUNT(s.Id)
                    AS TotalOrders,

                ISNULL(
                    SUM(s.TotalAmount),
                    0
                ) AS TotalRevenue

            FROM Customers c

            LEFT JOIN Sales s
                ON c.Id = s.CustomerId
                AND s.IsDeleted = 0

            WHERE
                c.TenantId = @TenantId
                AND c.IsDeleted = 0

            GROUP BY
                c.Id,
                c.Name,
                c.Email,
                c.PhoneNumber

            HAVING
                MAX(s.SaleDate) IS NULL

                OR

                MAX(s.SaleDate)
                    < DATEADD(
                        DAY,
                        -@Days,
                        GETUTCDATE()
                    )

            ORDER BY
                LastPurchaseDate ASC
            ";

            var result =
                await connection.QueryAsync<
                    InactiveCustomerDto>(
                    sql,
                    new
                    {
                        TenantId =
                            _currentTenant.TenantId,
                        request.Days
                    });

            return ApiResponse<
                List<InactiveCustomerDto>>
                .SuccessResponse(
                    result.ToList()
                );
        }
    }
}