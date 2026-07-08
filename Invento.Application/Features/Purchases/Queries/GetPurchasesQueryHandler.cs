using System.Data;
using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Purchases.DTOs;
using Invento.Application.Interfaces;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.Purchases.Queries
{
    public class GetPurchasesQueryHandler
        : IQueryHandler<
            GetPurchasesQuery,
            ApiResponse<PagedResponse<PurchaseDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetPurchasesQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<
            ApiResponse<PagedResponse<PurchaseDto>>> Handle(
            GetPurchasesQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await ((System.Data.Common.DbConnection)connection)
                    .OpenAsync(cancellationToken);
            }

            const string sql = """
                SELECT
                    p.Id,
                    p.SupplierId,
                    s.Name AS SupplierName,
                    p.PurchaseNumber,
                    p.PurchaseDate,
                    p.TotalAmount,
                    p.IsDeleted
                FROM Purchases p
                INNER JOIN Suppliers s
                    ON s.Id = p.SupplierId
                    AND s.TenantId = p.TenantId
                    AND s.IsDeleted = 0
                WHERE
                    p.TenantId = @TenantId
                    AND p.IsDeleted = 0
                    AND
                    (
                        @SupplierId IS NULL
                        OR p.SupplierId = @SupplierId
                    )
                    AND
                    (
                        @Search IS NULL
                        OR p.PurchaseNumber LIKE '%' + @Search + '%'
                        OR s.Name LIKE '%' + @Search + '%'
                    )
                    AND
                    (
                        @FromDate IS NULL
                        OR p.PurchaseDate >= @FromDate
                    )
                    AND
                    (
                        @ToDate IS NULL
                        OR p.PurchaseDate <= @ToDate
                    )
                ORDER BY
                    p.PurchaseDate DESC,
                    p.Id DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT_BIG(*)
                FROM Purchases p
                INNER JOIN Suppliers s
                    ON s.Id = p.SupplierId
                    AND s.TenantId = p.TenantId
                    AND s.IsDeleted = 0
                WHERE
                    p.TenantId = @TenantId
                    AND p.IsDeleted = 0
                    AND
                    (
                        @SupplierId IS NULL
                        OR p.SupplierId = @SupplierId
                    )
                    AND
                    (
                        @Search IS NULL
                        OR p.PurchaseNumber LIKE '%' + @Search + '%'
                        OR s.Name LIKE '%' + @Search + '%'
                    )
                    AND
                    (
                        @FromDate IS NULL
                        OR p.PurchaseDate >= @FromDate
                    )
                    AND
                    (
                        @ToDate IS NULL
                        OR p.PurchaseDate <= @ToDate
                    );
                """;

            var parameters = new
            {
                TenantId = _currentTenant.TenantId,
                Search =
                    string.IsNullOrWhiteSpace(request.Search)
                        ? null
                        : request.Search.Trim(),
                request.SupplierId,
                request.FromDate,
                request.ToDate,
                Offset =
                    checked(
                        (request.PageNumber - 1)
                        * request.PageSize),
                request.PageSize
            };

            var command =
                new CommandDefinition(
                    sql,
                    parameters,
                    commandTimeout: 30,
                    cancellationToken: cancellationToken);

            using var multi =
                await connection.QueryMultipleAsync(command);

            var purchases =
                (await multi.ReadAsync<PurchaseDto>())
                .ToList();

            var totalRecords =
                await multi.ReadSingleAsync<long>();

            return ApiResponse<PagedResponse<PurchaseDto>>
                .SuccessResponse(
                    new PagedResponse<PurchaseDto>
                    {
                        Items = purchases,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalCount =
                            checked((int)totalRecords)
                    });
        }
    }
}