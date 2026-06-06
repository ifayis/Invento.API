using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Customers.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Customers.Queries
{
    public class GetCustomerSalesSummaryQueryHandler
        : IQueryHandler<
            GetCustomerSalesSummaryQuery,
            ApiResponse<CustomerSalesSummaryDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetCustomerSalesSummaryQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<CustomerSalesSummaryDto>> Handle(
            GetCustomerSalesSummaryQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var summarySql = @"
            SELECT

                c.Id AS CustomerId,
                c.Name AS CustomerName,
                c.Email,
                c.PhoneNumber,
                c.Address,

                COUNT(s.Id) AS TotalOrders,

                ISNULL(
                    SUM(s.TotalAmount),
                    0
                ) AS TotalRevenue,

                ISNULL(
                    SUM(s.ProfitAmount),
                    0
                ) AS TotalProfit,

                MIN(s.SaleDate)
                    AS FirstPurchaseDate,

                MAX(s.SaleDate)
                    AS LastPurchaseDate

            FROM Customers c

            LEFT JOIN Sales s
                ON c.Id = s.CustomerId
                AND s.IsDeleted = 0

            WHERE
                c.Id = @CustomerId
                AND c.TenantId = @TenantId

            GROUP BY
                c.Id,
                c.Name,
                c.Email,
                c.PhoneNumber,
                c.Address
            ";

            var customer =
                await connection
                    .QueryFirstOrDefaultAsync<
                        CustomerSalesSummaryDto>(
                        summarySql,
                        new
                        {
                            request.CustomerId,
                            TenantId =
                                _currentTenant.TenantId
                        });

            if (customer is null)
            {
                return ApiResponse<
                    CustomerSalesSummaryDto>
                    .FailureResponse(
                        new()
                        {
                            "Customer not found"
                        });
            }

            var salesSql = @"
            SELECT TOP 10

                Id AS SaleId,
                InvoiceNumber,
                SaleDate,
                TotalAmount,
                ProfitAmount

            FROM Sales

            WHERE
                CustomerId = @CustomerId
                AND TenantId = @TenantId
                AND IsDeleted = 0

            ORDER BY
                SaleDate DESC
            ";

            var sales =
                await connection
                    .QueryAsync<
                        CustomerSaleHistoryDto>(
                        salesSql,
                        new
                        {
                            request.CustomerId,
                            TenantId =
                                _currentTenant.TenantId
                        });

            customer.RecentSales =
                sales.ToList();

            return ApiResponse<
                CustomerSalesSummaryDto>
                .SuccessResponse(customer);
        }
    }
}