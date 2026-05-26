using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Sales.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Sales.Queries
{
    public class GetCustomerSalesQueryHandler
        : IQueryHandler<
        GetCustomerSalesQuery,
        ApiResponse<CustomerSalesSummaryDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetCustomerSalesQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<
            ApiResponse<CustomerSalesSummaryDto>>
            Handle(
            GetCustomerSalesQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
            SELECT
                c.Id CustomerId,
                c.Name CustomerName,
                COUNT(s.Id) TotalOrders,
                ISNULL(SUM(s.TotalAmount),0)
                    TotalSalesAmount,
                ISNULL(SUM(s.ProfitAmount),0)
                    TotalProfit

            FROM Customers c

            LEFT JOIN Sales s
            ON c.Id=s.CustomerId

            WHERE c.Id=@CustomerId
            AND c.TenantId=@TenantId

            GROUP BY
                c.Id,
                c.Name
            ";

            var result =
                await connection.QueryFirstOrDefaultAsync
                <CustomerSalesSummaryDto>(
                    sql,
                    new
                    {
                        request.CustomerId,
                        TenantId =
                        _currentTenant.TenantId
                    });

            if (result is null)
            {
                return ApiResponse<
                    CustomerSalesSummaryDto>
                    .FailureResponse(
                    new()
                    {
                        "Customer not found"
                    });
            }

            return ApiResponse<
                CustomerSalesSummaryDto>
                .SuccessResponse(result);
        }
    }
}