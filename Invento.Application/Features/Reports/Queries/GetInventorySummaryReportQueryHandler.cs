using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Reports.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetInventorySummaryReportQueryHandler
    : IQueryHandler<
    GetInventorySummaryReportQuery,
    ApiResponse<InventorySummaryReportDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

    public GetInventorySummaryReportQueryHandler(
        IDbConnectionFactory connectionFactory,
        ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<InventorySummaryReportDto>> Handle(
            GetInventorySummaryReportQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
        SELECT
            COUNT(*) AS TotalProducts,
            ISNULL(SUM(CurrentStock),0) AS TotalStockQuantity,
            ISNULL(SUM(CurrentStock * CostPrice),0) AS InventoryValue,
            COUNT(
                CASE
                    WHEN CurrentStock <= LowStockThreshold
                    THEN 1
                END
            ) AS LowStockProducts
        FROM Products
        WHERE
            TenantId = @TenantId
            AND IsDeleted = 0;
        ";

            var result =
                await connection.QueryFirstAsync<InventorySummaryReportDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId
                    });

            return ApiResponse<InventorySummaryReportDto>
                .SuccessResponse(result);
        }
    }
}
