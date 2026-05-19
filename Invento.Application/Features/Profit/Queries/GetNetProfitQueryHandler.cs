using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Profit.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Profit.Queries;

public class GetNetProfitQueryHandler
    : IQueryHandler<
        GetNetProfitQuery,
        ApiResponse<ProfitSummaryDto>>
{
    private readonly IDbConnectionFactory
        _connectionFactory;

    private readonly ICurrentTenantService
        _currentTenant;

    public GetNetProfitQueryHandler(
        IDbConnectionFactory connectionFactory,
        ICurrentTenantService currentTenant)
    {
        _connectionFactory = connectionFactory;
        _currentTenant = currentTenant;
    }

    public async Task<
        ApiResponse<ProfitSummaryDto>>
        Handle(
            GetNetProfitQuery request,
            CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var sql = @"
        SELECT
            ISNULL(SUM(TotalAmount), 0)
                AS TotalRevenue,

            ISNULL(SUM(ProfitAmount), 0)
                AS TotalProfit,

            COUNT(*) AS TotalSales

        FROM Sales

        WHERE
            IsDeleted = 0
            AND TenantId = @TenantId
        ";

        var result =
            await connection
            .QueryFirstAsync<ProfitSummaryDto>(
                sql,
                new
                {
                    TenantId =
                        _currentTenant.TenantId
                });

        return ApiResponse<ProfitSummaryDto>
            .SuccessResponse(result);
    }
}