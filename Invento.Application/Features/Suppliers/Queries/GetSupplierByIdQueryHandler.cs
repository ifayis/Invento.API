using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Suppliers.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Suppliers.Queries
{
    public class GetSupplierByIdQueryHandler
        : IQueryHandler<
            GetSupplierByIdQuery,
            ApiResponse<SupplierDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetSupplierByIdQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<SupplierDto>> Handle(
            GetSupplierByIdQuery request,
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
                Id = @Id
                AND TenantId = @TenantId
            ";

            var supplier =
                await connection.QueryFirstOrDefaultAsync<SupplierDto>(
                    sql,
                    new
                    {
                        request.Id,
                        TenantId = _currentTenant.TenantId
                    });

            if (supplier is null)
            {
                return ApiResponse<SupplierDto>
                    .FailureResponse(
                        new()
                        {
                            "Supplier not found"
                        });
            }

            return ApiResponse<SupplierDto>
                .SuccessResponse(supplier);
        }
    }
}