using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Reports.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetTopSellingProductsReportQueryHandler
        : IQueryHandler<
            GetTopSellingProductsReportQuery,
            ApiResponse<List<TopSellingProductReportDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetTopSellingProductsReportQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<TopSellingProductReportDto>>> Handle(
            GetTopSellingProductsReportQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
            SELECT TOP (@Top)
                p.Id AS ProductId,
                p.Name AS ProductName,
                SUM(si.Quantity) AS TotalQuantitySold,
                SUM(si.TotalPrice) AS TotalRevenue,
                SUM(si.ProfitAmount) AS TotalProfit
            FROM SaleItems si
            INNER JOIN Sales s
                ON si.SaleId = s.Id
            INNER JOIN Products p
                ON si.ProductId = p.Id
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
                p.Id,
                p.Name
            ORDER BY
                SUM(si.Quantity) DESC;
            ";

            var result =
                (await connection.QueryAsync<TopSellingProductReportDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId,
                        request.Top,
                        request.FromDate,
                        request.ToDate
                    }))
                .ToList();

            return ApiResponse<List<TopSellingProductReportDto>>
                .SuccessResponse(result);
        }
    }
}