using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Customers.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Customers.Queries
{
    public class GetTopCustomersQueryHandler
        : IQueryHandler<
            GetTopCustomersQuery,
            ApiResponse<List<CustomerSalesSummaryDto>>>
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

        public async Task<ApiResponse<List<CustomerSalesSummaryDto>>>
            Handle(
                GetTopCustomersQuery request,
                CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
            SELECT TOP (@Top)

                c.Id AS CustomerId,

                c.Name AS CustomerName,

                c.Email,

                c.PhoneNumber,

                COUNT(s.Id)
                    AS TotalOrders,

                ISNULL(
                    SUM(s.TotalAmount),
                    0
                ) AS TotalRevenue,

                ISNULL(
                    SUM(s.ProfitAmount),
                    0
                ) AS TotalProfit,

                MAX(s.SaleDate)
                    AS LastPurchaseDate

            FROM Customers c

            INNER JOIN Sales s
                ON c.Id = s.CustomerId

            WHERE
                c.TenantId = @TenantId
                AND c.IsDeleted = 0
                AND s.IsDeleted = 0

            GROUP BY
                c.Id,
                c.Name,
                c.Email,
                c.PhoneNumber

            ORDER BY
                SUM(s.TotalAmount) DESC
            ";

            var result =
                await connection.QueryAsync<
                    CustomerSalesSummaryDto>(
                    sql,
                    new
                    {
                        TenantId =
                            _currentTenant.TenantId,
                        request.Top
                    });

            return ApiResponse<
                List<CustomerSalesSummaryDto>>
                .SuccessResponse(
                    result.ToList()
                );
        }
    }
}