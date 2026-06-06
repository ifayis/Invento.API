using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.StockMovements.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Enums;

namespace Invento.Application.Features.StockMovements.Queries
{
    public class GetInventoryDashboardQueryHandler
        : IQueryHandler<
            GetInventoryDashboardQuery,
            ApiResponse<InventoryDashboardDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetInventoryDashboardQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<InventoryDashboardDto>> Handle(
            GetInventoryDashboardQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
            SELECT

            ISNULL(
            (
                SELECT SUM(Quantity)
                FROM StockMovements
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                    AND MovementType IN
                    (
                        'Purchase',
                        'AdjustmentIn',
                        'Return',
                        'SaleRestore'
                    )
            ),0) AS TotalStockIn,

            ISNULL(
            (
                SELECT SUM(Quantity)
                FROM StockMovements
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                    AND MovementType IN
                    (
                        'Sale',
                        'AdjustmentOut'
                    )
            ),0) AS TotalStockOut,

            ISNULL(
            (
                SELECT SUM(Quantity)
                FROM StockMovements
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                    AND MovementType IN
                    (
                        'AdjustmentIn',
                        'AdjustmentOut'
                    )
            ),0) AS TotalAdjustments,

            (
                SELECT COUNT(*)
                FROM Products
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
            ) AS TotalProducts,

            (
                SELECT COUNT(*)
                FROM Products
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                    AND CurrentStock <= LowStockThreshold
            ) AS LowStockProducts,

            (
                SELECT COUNT(*)
                FROM Products p
                INNER JOIN TenantSettings ts
                    ON p.TenantId = ts.TenantId
                WHERE
                    p.TenantId = @TenantId
                    AND p.IsDeleted = 0
                    AND p.CurrentStock
                        <= ts.CriticalStockThreshold
            ) AS CriticalStockProducts
            ";

            var result =
                await connection.QueryFirstAsync<
                    InventoryDashboardDto>(
                        sql,
                        new
                        {
                            TenantId =
                                _currentTenant.TenantId
                        });

            return ApiResponse<InventoryDashboardDto>
                .SuccessResponse(result);
        }
    }
}