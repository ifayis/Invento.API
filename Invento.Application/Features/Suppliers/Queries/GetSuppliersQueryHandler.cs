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

        public async Task<ApiResponse<PagedResponse<SupplierDto>>> Handle(
            GetSuppliersQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
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
                AND
                (
                    @Search IS NULL
                    OR Name LIKE '%' + @Search + '%'
                    OR ContactPerson LIKE '%' + @Search + '%'
                    OR Email LIKE '%' + @Search + '%'
                    OR PhoneNumber LIKE '%' + @Search + '%'
                )

            ORDER BY CreatedAt DESC

            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;

            SELECT COUNT(*)
            FROM Suppliers
            WHERE
                TenantId = @TenantId
                AND
                (
                    @Search IS NULL
                    OR Name LIKE '%' + @Search + '%'
                    OR ContactPerson LIKE '%' + @Search + '%'
                    OR Email LIKE '%' + @Search + '%'
                    OR PhoneNumber LIKE '%' + @Search + '%'
                );
            ";

            var parameters = new
            {
                TenantId = _currentTenant.TenantId,
                request.Search,
                Offset =
                    (request.PageNumber - 1)
                    * request.PageSize,
                request.PageSize
            };

            using var multi =
                await connection.QueryMultipleAsync(
                    sql,
                    parameters);

            var suppliers =
                await multi.ReadAsync<SupplierDto>();

            var totalRecords =
                await multi.ReadFirstAsync<int>();

            var response =
                new PagedResponse<SupplierDto>
                {
                    Items = suppliers.ToList(),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalRecords
                };

            return ApiResponse<PagedResponse<SupplierDto>>
                .SuccessResponse(response);
        }
    }
}