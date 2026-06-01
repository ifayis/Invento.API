using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Profit.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Profit.Queries
{
    public class GetDashboardSummaryQueryHandler
        : IQueryHandler<GetDashboardSummaryQuery, ApiResponse<DashboardSummaryDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetDashboardSummaryQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task< ApiResponse<DashboardSummaryDto>> Handle(
                GetDashboardSummaryQuery request,
                CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
            SELECT

            (
                SELECT ISNULL(SUM(TotalAmount),0)
                FROM Sales
                WHERE
                    IsDeleted = 0
                    AND TenantId=@TenantId
            ) AS TotalRevenue,

            (
                SELECT ISNULL(SUM(ProfitAmount),0)
                FROM Sales
                WHERE
                    IsDeleted = 0
                    AND TenantId=@TenantId
            ) AS TotalProfit,

            (
                SELECT COUNT(*)
                FROM Sales
                WHERE
                    IsDeleted = 0
                    AND TenantId=@TenantId
            ) AS TotalSales,

            (
                SELECT COUNT(*)
                FROM Products
                WHERE
                    IsDeleted = 0
                    AND TenantId=@TenantId
            ) AS TotalProducts,

            (
                SELECT COUNT(*)
                FROM Products
                WHERE
                    IsDeleted = 0
                    AND TenantId=@TenantId
                    AND CurrentStock <= LowStockThreshold
            ) AS LowStockProducts
            ";

            var result = await connection
                .QueryFirstAsync<DashboardSummaryDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId
                    }
                );

            return ApiResponse<DashboardSummaryDto>
                .SuccessResponse(result);
        }
    }
}