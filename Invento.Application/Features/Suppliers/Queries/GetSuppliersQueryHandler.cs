using System.Data;
using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Suppliers.DTOs;
using Invento.Application.Interfaces;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.Suppliers.Queries
{
    public class GetSuppliersQueryHandler
        : IQueryHandler<
            GetSuppliersQuery,
            ApiResponse<PagedResponse<SupplierDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetSuppliersQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<
            ApiResponse<PagedResponse<SupplierDto>>> Handle(
            GetSuppliersQuery request,
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
                    Id,
                    Name,
                    ContactPerson,
                    Email,
                    PhoneNumber,
                    Address,
                    TaxRegistrationNumber,
                    IsDeleted,
                    CreatedAt
                FROM Suppliers
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                    AND
                    (
                        @Search IS NULL
                        OR Name LIKE '%' + @Search + '%'
                        OR ContactPerson LIKE '%' + @Search + '%'
                        OR Email LIKE '%' + @Search + '%'
                        OR PhoneNumber LIKE '%' + @Search + '%'
                    )
                ORDER BY
                    CreatedAt DESC,
                    Id DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT_BIG(*)
                FROM Suppliers
                WHERE
                    TenantId = @TenantId
                    AND IsDeleted = 0
                    AND
                    (
                        @Search IS NULL
                        OR Name LIKE '%' + @Search + '%'
                        OR ContactPerson LIKE '%' + @Search + '%'
                        OR Email LIKE '%' + @Search + '%'
                        OR PhoneNumber LIKE '%' + @Search + '%'
                    );
                """;

            var parameters = new
            {
                TenantId = _currentTenant.TenantId,

                Search =
                    string.IsNullOrWhiteSpace(request.Search)
                        ? null
                        : request.Search.Trim(),

                Offset = checked(
                    (request.PageNumber - 1)
                    * request.PageSize),

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

            var suppliers =
                (await multi.ReadAsync<SupplierDto>())
                .ToList();

            var totalRecords =
                await multi.ReadSingleAsync<long>();

            return ApiResponse<
                PagedResponse<SupplierDto>>
                .SuccessResponse(
                    new PagedResponse<SupplierDto>
                    {
                        Items = suppliers,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize,
                        TotalCount =
                            checked((int)totalRecords)
                    });
        }
    }
}