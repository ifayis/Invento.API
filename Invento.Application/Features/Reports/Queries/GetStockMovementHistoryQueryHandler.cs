using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Interfaces;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetStockMovementHistoryQueryHandler
    : IQueryHandler<
    GetStockMovementHistoryQuery,
    ApiResponse<PagedResponse<StockMovementDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

    public GetStockMovementHistoryQueryHandler(
        IDbConnectionFactory connectionFactory,
        ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<PagedResponse<StockMovementDto>>> Handle(
            GetStockMovementHistoryQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
        SELECT
            sm.Id,
            sm.ProductId,
            p.Name AS ProductName,
            sm.Quantity,
            sm.MovementType,
            sm.CurrentStockAfterMovement,
            sm.Remarks,
            sm.ReferenceNumber,
            sm.CreatedByUserId,
            sm.CreatedAt
        FROM StockMovements sm
        INNER JOIN Products p
            ON sm.ProductId = p.Id
        WHERE
            sm.TenantId = @TenantId
            AND
            (
                @ProductId IS NULL
                OR sm.ProductId = @ProductId
            )

        ORDER BY sm.CreatedAt DESC

        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(*)
        FROM StockMovements
        WHERE
            TenantId = @TenantId
            AND
            (
                @ProductId IS NULL
                OR ProductId = @ProductId
            );
        ";

            var parameters = new
            {
                TenantId = _currentTenant.TenantId,
                request.ProductId,
                Offset =
                    (request.PageNumber - 1)
                    * request.PageSize,
                request.PageSize
            };

            using var multi =
                await connection.QueryMultipleAsync(
                    sql,
                    parameters);

            var movements =
                (await multi.ReadAsync<StockMovementDto>())
                .ToList();

            var totalRecords =
                await multi.ReadFirstAsync<int>();

            var response =
                new PagedResponse<StockMovementDto>
                {
                    Items = movements,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalRecords
                };

            return ApiResponse<PagedResponse<StockMovementDto>>
                .SuccessResponse(response);
        }
    }

}
