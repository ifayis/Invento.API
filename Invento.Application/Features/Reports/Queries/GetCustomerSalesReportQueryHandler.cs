using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Reports.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetCustomerSalesReportQueryHandler
        : IQueryHandler<
            GetCustomerSalesReportQuery,
            ApiResponse<List<CustomerSalesReportDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetCustomerSalesReportQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<CustomerSalesReportDto>>> Handle(
            GetCustomerSalesReportQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
            SELECT
                c.Id AS CustomerId,
                c.Name AS CustomerName,
                COUNT(s.Id) AS TotalOrders,
                ISNULL(SUM(s.TotalAmount),0) AS TotalSales,
                ISNULL(SUM(s.ProfitAmount),0) AS TotalProfit
            FROM Customers c
            INNER JOIN Sales s
                ON c.Id = s.CustomerId
            WHERE
                s.TenantId = @TenantId
                AND s.IsDeleted = 0
                AND
                (
                    @FromDate IS NULL
                    OR s.SaleDate >= @FromDate
                )
                AND
                (
                    @ToDate IS NULL
                    OR s.SaleDate <= @ToDate
                )
            GROUP BY
                c.Id,
                c.Name
            ORDER BY
                TotalSales DESC;
            ";

            var result =
                (await connection.QueryAsync<CustomerSalesReportDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId,
                        request.FromDate,
                        request.ToDate
                    }))
                .ToList();

            return ApiResponse<List<CustomerSalesReportDto>>
                .SuccessResponse(result);
        }
    }
}