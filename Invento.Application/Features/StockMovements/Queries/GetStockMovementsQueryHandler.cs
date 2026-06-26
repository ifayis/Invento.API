using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Interfaces;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.StockMovements.Queries
{
    public class GetStockMovementsQueryHandler
        : IQueryHandler<GetStockMovementsQuery, ApiResponse<PagedResponse<StockMovementDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;


        public GetStockMovementsQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<PagedResponse<StockMovementDto>>> Handle(
            GetStockMovementsQuery request,
            CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateConnection();

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
                sm.CreatedByUserId
            FROM StockMovements sm
            INNER JOIN Products p
                ON sm.ProductId = p.Id
            WHERE sm.IsDeleted = 0
            AND sm.TenantId = @TenantId
            AND
            (
                @ProductId IS NULL
                OR sm.ProductId = @ProductId
            )
            AND
            (
                @MovementType IS NULL
                OR sm.MovementType = @MovementType
            )
            AND
            (
                @Search IS NULL
                OR p.Name LIKE '%' + @Search + '%'
            )
            AND
            (
                @FromDate IS NULL
                OR sm.CreatedAt >= @FromDate
            )
            AND
            (
                @ToDate IS NULL
                OR sm.CreatedAt <= @ToDate
            )
            ORDER BY sm.Id DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;

            SELECT COUNT(*)
            FROM StockMovements sm
            INNER JOIN Products p
                ON sm.ProductId = p.Id
            WHERE sm.IsDeleted = 0
            AND sm.TenantId = @TenantId
            AND
            (
                @ProductId IS NULL
                OR sm.ProductId = @ProductId
            )
            AND
            (
                @MovementType IS NULL
                OR sm.MovementType = @MovementType
            )
            AND
            (
                @Search IS NULL
                OR p.Name LIKE '%' + @Search + '%'
            )
            AND
            (
                @FromDate IS NULL
                OR sm.CreatedAt >= @FromDate
            )
            AND
            (
                @ToDate IS NULL
                OR sm.CreatedAt <= @ToDate
            );
            ";

            var parameters = new
            {
                TenantId = _currentTenant.TenantId,
                request.ProductId,
                request.Search,
                request.MovementType,
                request.FromDate,
                request.ToDate,
                Offset =
                    (request.PageNumber - 1)
                    * request.PageSize,
                request.PageSize
            };

            using var multi = await connection.QueryMultipleAsync(
                    sql,
                    parameters
            );

            var items = await multi.ReadAsync<StockMovementDto>();

            var totalRecords = await multi.ReadFirstAsync<int>();

            var response =
                new PagedResponse<StockMovementDto>
                {
                    Items = items.ToList(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalRecords
                };

            return ApiResponse<PagedResponse<StockMovementDto>>
                .SuccessResponse(response);
        }
    }
}