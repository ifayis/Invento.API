using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Dashboard.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Dashboard.Queries
{
    public class GetSalesTrendQueryHandler
        : IQueryHandler<
            GetSalesTrendQuery,
            ApiResponse<List<SalesTrendDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetSalesTrendQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<SalesTrendDto>>> Handle(
            GetSalesTrendQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
            SELECT
                FORMAT(
                    SaleDate,
                    'MMM yyyy'
                ) AS Month,

                SUM(TotalAmount)
                    AS SalesAmount,

                SUM(ProfitAmount)
                    AS ProfitAmount

            FROM Sales

            WHERE
                TenantId = @TenantId
                AND IsDeleted = 0

            GROUP BY
                YEAR(SaleDate),
                MONTH(SaleDate),
                FORMAT(SaleDate,'MMM yyyy')

            ORDER BY
                YEAR(SaleDate),
                MONTH(SaleDate);
            ";

            var result =
                (await connection.QueryAsync<SalesTrendDto>(
                    sql,
                    new
                    {
                        TenantId =
                            _currentTenant.TenantId
                    }))
                .ToList();

            return ApiResponse<List<SalesTrendDto>>
                .SuccessResponse(result);
        }
    }
}