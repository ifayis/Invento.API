using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Reports.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetSalesSummaryReportQueryHandler
        : IQueryHandler<
            GetSalesSummaryReportQuery,
            ApiResponse<SalesSummaryReportDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetSalesSummaryReportQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<SalesSummaryReportDto>> Handle(
            GetSalesSummaryReportQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
            SELECT
                ISNULL(SUM(TotalAmount),0) AS TotalSales,
                ISNULL(SUM(ProfitAmount),0) AS TotalProfit,
                COUNT(*) AS TotalOrders,
                ISNULL(AVG(TotalAmount),0) AS AverageOrderValue
            FROM Sales
            WHERE
                TenantId = @TenantId
                AND IsDeleted = 0
                AND
                (
                    @FromDate IS NULL
                    OR SaleDate >= @FromDate
                )
                AND
                (
                    @ToDate IS NULL
                    OR SaleDate <= @ToDate
                );
            ";

            var result =
                await connection.QueryFirstAsync<SalesSummaryReportDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId,
                        request.FromDate,
                        request.ToDate
                    });

            return ApiResponse<SalesSummaryReportDto>
                .SuccessResponse(result);
        }
    }
}