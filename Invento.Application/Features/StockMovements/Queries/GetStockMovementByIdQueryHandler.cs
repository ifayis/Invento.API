using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.StockMovements.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.StockMovements.Queries;

public class GetStockMovementByIdQueryHandler
    : IQueryHandler<
        GetStockMovementByIdQuery,
        ApiResponse<StockMovementDto>>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetStockMovementByIdQueryHandler(
        IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ApiResponse<StockMovementDto>> Handle(
        GetStockMovementByIdQuery request,
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
            sm.Remarks,
            sm.ReferenceNumber,
            sm.CreatedAt
        FROM StockMovements sm
        INNER JOIN Products p
            ON sm.ProductId = p.Id
        WHERE sm.Id = @Id
        AND sm.IsDeleted = 0
        ";

        var movement =
            await connection.QueryFirstOrDefaultAsync
            <StockMovementDto>(
                sql,
                new { request.Id });

        if (movement is null)
        {
            return ApiResponse<StockMovementDto>
                .FailureResponse(
                    new List<string>
                    {
                        "Stock movement not found"
                    });
        }

        return ApiResponse<StockMovementDto>
            .SuccessResponse(movement);
    }
}