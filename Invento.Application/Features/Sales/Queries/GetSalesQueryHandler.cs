using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Sales.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Sales.Queries;

public class GetSalesQueryHandler
    : IQueryHandler<
        GetSalesQuery,
        ApiResponse<PagedResponse<SaleDto>>>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ICurrentTenantService _currentTenant;

    public GetSalesQueryHandler(
        IDbConnectionFactory connectionFactory,
        ICurrentTenantService currentTenant)
    {
        _connectionFactory = connectionFactory;
        _currentTenant = currentTenant;
    }

    public async Task<
        ApiResponse<PagedResponse<SaleDto>>>
        Handle(
            GetSalesQuery request,
            CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var sql = @"
        SELECT
            Id,
            InvoiceNumber,
            SaleDate,
            TotalAmount,
            ProfitAmount,
            CreatedAt
        FROM Sales
        WHERE IsDeleted = 0
        AND TenantId =@TenantId
        AND
        (
            @Search IS NULL
            OR InvoiceNumber
                LIKE '%' + @Search + '%'
        )

        AND
        (
            @FromDate IS NULL
            OR SaleDate >= @FromDate
        )

        AND
        (
            @ToDate IS NULL
            OR SaleDate <= @ToDate
        )

        ORDER BY SaleDate DESC

        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(*)
        FROM Sales
        WHERE IsDeleted = 0
        AND TenantId = @TenantId
        AND
        (
            @Search IS NULL
            OR InvoiceNumber
                LIKE '%' + @Search + '%'
        )

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

        var parameters = new
        {
            TenantId = _currentTenant.TenantId,

            request.Search,

            request.FromDate,

            request.ToDate,

            Offset =
                (request.PageNumber - 1)
                * request.PageSize,

            request.PageSize
        };

        using var multi =
            await connection.QueryMultipleAsync(
                sql,
                parameters);

        var sales =
            await multi.ReadAsync<SaleDto>();

        var totalRecords =
            await multi.ReadFirstAsync<int>();

        var response =
            new PagedResponse<SaleDto>
            {
                Items = sales,

                PageNumber =
                    request.PageNumber,

                PageSize =
                    request.PageSize,

                TotalRecords =
                    totalRecords
            };

        return ApiResponse<
            PagedResponse<SaleDto>>
            .SuccessResponse(response);
    }
}