using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Interfaces;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.StockMovements.Queries
{
    public class GetStockMovementsQueryHandler
        : IQueryHandler<
            GetStockMovementsQuery,
            ApiResponse<PagedResponse<StockMovementDto>>>
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
            using var connection =
                _connectionFactory.CreateConnection();

            var search =
                string.IsNullOrWhiteSpace(request.Search)
                    ? null
                    : request.Search.Trim();

            var movementType =
                string.IsNullOrWhiteSpace(request.MovementType)
                    ? null
                    : request.MovementType.Trim();

            const string sql = """
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
                    ON p.Id = sm.ProductId
                    AND p.TenantId = @TenantId
                WHERE
                    sm.TenantId = @TenantId
                    AND sm.IsDeleted = 0
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
                ORDER BY
                    sm.CreatedAt DESC,
                    sm.Id DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(*)
                FROM StockMovements sm
                INNER JOIN Products p
                    ON p.Id = sm.ProductId
                    AND p.TenantId = @TenantId
                WHERE
                    sm.TenantId = @TenantId
                    AND sm.IsDeleted = 0
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
                """;

            var parameters = new
            {
                TenantId = _currentTenant.TenantId,
                request.ProductId,
                Search = search,
                MovementType = movementType,
                request.FromDate,
                request.ToDate,
                Offset =
                    (request.PageNumber - 1)
                    * request.PageSize,
                request.PageSize
            };

            var command =
                new CommandDefinition(
                    sql,
                    parameters,
                    cancellationToken:
                        cancellationToken);

            using var multi =
                await connection.QueryMultipleAsync(
                    command);

            var items =
                (await multi.ReadAsync<StockMovementDto>())
                .ToList();

            var totalRecords =
                await multi.ReadSingleAsync<int>();

            var response =
                new PagedResponse<StockMovementDto>
                {
                    Items = items,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalRecords
                };

            return ApiResponse<
                PagedResponse<StockMovementDto>>
                .SuccessResponse(response);
        }
    }
}