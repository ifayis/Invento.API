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
                    sm.CreatedByUserId,
                    sm.CreatedAt
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
                ORDER BY
                    sm.CreatedAt DESC,
                    sm.Id DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(*)
                FROM StockMovements sm
                WHERE
                    sm.TenantId = @TenantId
                    AND sm.IsDeleted = 0
                    AND
                    (
                        @ProductId IS NULL
                        OR sm.ProductId = @ProductId
                    );
                """;

            var parameters = new
            {
                TenantId = _currentTenant.TenantId,
                request.ProductId,
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

            var movements =
                (await multi.ReadAsync<StockMovementDto>())
                .ToList();

            var totalRecords =
                await multi.ReadSingleAsync<int>();

            var response =
                new PagedResponse<StockMovementDto>
                {
                    Items = movements,
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