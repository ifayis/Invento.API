using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Company.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Company.Queries
{
    public class GetCompanyProfileQueryHandler
        : IQueryHandler<GetCompanyProfileQuery, ApiResponse<CompanyProfileDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetCompanyProfileQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<
            ApiResponse<CompanyProfileDto>> Handle(
                GetCompanyProfileQuery request,
                CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
        SELECT
            Id,
            CompanyName,
            Email,
            PhoneNumber,
            Address,
            LogoUrl,
            TaxNumber,
            Website

        FROM Tenants

        WHERE
            Id = @TenantId
            AND IsDeleted = 0
        ";

            var result = await connection.QueryFirstOrDefaultAsync
                <CompanyProfileDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId
                    }
                );

            if (result is null)
            {
                return ApiResponse<
                    CompanyProfileDto>
                    .FailureResponse(
                        new List<string>
                        {
                        "Company not found"
                        }
                    );
            }

            return ApiResponse<
                CompanyProfileDto>
                .SuccessResponse(result);
        }
    }
}