using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Sales.DTOs;
using Invento.Application.Interfaces;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.Sales.Queries
{
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

        public async Task<ApiResponse<PagedResponse<SaleDto>>> Handle(
            GetSalesQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var search =
                string.IsNullOrWhiteSpace(request.Search)
                    ? null
                    : request.Search.Trim();

            const string sql = """
            SELECT
                s.Id,
                s.CustomerId,
                c.Name AS CustomerName,
                s.InvoiceNumber,
                s.SaleDate,
                s.TotalAmount,
                s.ProfitAmount,
                s.IsDeleted
            FROM Sales s
            LEFT JOIN Customers c
                ON c.Id = s.CustomerId
                AND c.TenantId = s.TenantId
            WHERE
                s.TenantId = @TenantId
                AND s.IsDeleted = 0
                AND
                (
                    @Search IS NULL
                    OR s.InvoiceNumber LIKE '%' + @Search + '%'
                    OR c.Name LIKE '%' + @Search + '%'
                )
                AND
                (
                    @FromDate IS NULL
                    OR s.SaleDate >= @FromDate
                )
                AND
                (
                    @ToDate IS NULL
                    OR s.SaleDate <= @ToDate
                )
            ORDER BY
                s.SaleDate DESC,
                s.Id DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;

            SELECT COUNT(*)
            FROM Sales s
            LEFT JOIN Customers c
                ON c.Id = s.CustomerId
                AND c.TenantId = s.TenantId
            WHERE
                s.TenantId = @TenantId
                AND s.IsDeleted = 0
                AND
                (
                    @Search IS NULL
                    OR s.InvoiceNumber LIKE '%' + @Search + '%'
                    OR c.Name LIKE '%' + @Search + '%'
                )
                AND
                (
                    @FromDate IS NULL
                    OR s.SaleDate >= @FromDate
                )
                AND
                (
                    @ToDate IS NULL
                    OR s.SaleDate <= @ToDate
                );
            """;

            var parameters = new
            {
                TenantId = _currentTenant.TenantId,
                Search = search,
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

            var sales =
                (await multi.ReadAsync<SaleDto>())
                .ToList();

            var totalRecords =
                await multi.ReadSingleAsync<int>();

            var response =
                new PagedResponse<SaleDto>
                {
                    Items = sales,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalRecords
                };

            return ApiResponse<
                PagedResponse<SaleDto>>
                .SuccessResponse(response);
        }
    }
}