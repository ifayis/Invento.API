using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Dashboard.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Dashboard.Queries
{
    public class GetRecentPurchasesQueryHandler
    : IQueryHandler<
    GetRecentPurchasesQuery,
    ApiResponse<List<RecentPurchaseDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetRecentPurchasesQueryHandler(
        IDbConnectionFactory connectionFactory,
        ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<RecentPurchaseDto>>> Handle(
            GetRecentPurchasesQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
        SELECT TOP (@Count)
            Id,
            PurchaseNumber,
            PurchaseDate,
            TotalAmount
        FROM Purchases
        WHERE
            TenantId = @TenantId
            AND IsDeleted = 0
        ORDER BY PurchaseDate DESC;
        ";

            var purchases =
                (await connection.QueryAsync<RecentPurchaseDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId,
                        request.Count
                    }))
                .ToList();

            return ApiResponse<List<RecentPurchaseDto>>
                .SuccessResponse(purchases);
        }
    }

}
