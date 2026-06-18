using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Dashboard.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Dashboard.Queries
{
    public class GetRecentSalesQueryHandler
    : IQueryHandler<
    GetRecentSalesQuery,
    ApiResponse<List<RecentSaleDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetRecentSalesQueryHandler(
        IDbConnectionFactory connectionFactory,
        ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<RecentSaleDto>>> Handle(
            GetRecentSalesQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
        SELECT TOP (@Count)
            Id,
            InvoiceNumber,
            SaleDate,
            TotalAmount
        FROM Sales
        WHERE
            TenantId = @TenantId
            AND IsDeleted = 0
        ORDER BY SaleDate DESC;
        ";

            var sales =
                (await connection.QueryAsync<RecentSaleDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId,
                        request.Count
                    }))
                .ToList();

            return ApiResponse<List<RecentSaleDto>>
                .SuccessResponse(sales);
        }
    }

}
