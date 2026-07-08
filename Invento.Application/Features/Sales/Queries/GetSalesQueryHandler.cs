using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Sales.DTOs;
using Invento.Application.Interfaces;
using Invento.Shared.Pagination;
using System.Data;

namespace Invento.Application.Features.Sales.Queries
{
    public class GetSalesQueryHandler
        : IQueryHandler<GetSalesQuery, ApiResponse<PagedResponse<SaleDto>>>
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

        public async Task<ApiResponse<PagedResponse<SaleDto>>> Handle(
                GetSalesQuery request,
                CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await ((System.Data.Common.DbConnection)connection)
                    .OpenAsync(cancellationToken);
            }

            var sql = @"
            SELECT
                Id,
                CustomerId,
                InvoiceNumber,
                SaleDate,
                TotalAmount,
                ProfitAmount,
                IsDeleted
            FROM Sales
            WHERE TenantId = @TenantId

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
            WHERE TenantId = @TenantId

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

            var command =
                new CommandDefinition(
                    commandText: sql,
                    parameters: parameters,
                    commandTimeout: 30,
                    cancellationToken: cancellationToken);

            using var multi =
                await connection.QueryMultipleAsync(command);

            var sales = await multi.ReadAsync<SaleDto>();

            var totalRecords = await multi.ReadFirstAsync<int>();

            var response =
                new PagedResponse<SaleDto>
                {
                    Items = sales.ToList(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalRecords
                };

            return ApiResponse<PagedResponse<SaleDto>>
                .SuccessResponse(response);
        }
    }
}