using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Profit.Queries
{
    public class GetProductProfitQueryHandler
        : IQueryHandler<GetProductProfitQuery, ApiResponse<decimal>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetProductProfitQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<decimal>> Handle(
                GetProductProfitQuery request,
                CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
            SELECT
                ISNULL(SUM(si.ProfitAmount), 0)
            FROM SaleItems si

            INNER JOIN Sales s
                ON si.SaleId = s.Id

            WHERE
                si.ProductId = @ProductId
                AND s.IsDeleted = 0
                AND s.TenantId = @TenantId
            ";

            var profit = await connection.ExecuteScalarAsync<decimal>(
                    sql,
                    new
                    {
                        request.ProductId,
                        TenantId = _currentTenant.TenantId
                    }
            );

            return ApiResponse<decimal>
                .SuccessResponse(profit);
        }
    }
}