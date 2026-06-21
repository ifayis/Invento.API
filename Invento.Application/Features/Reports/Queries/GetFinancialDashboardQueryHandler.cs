using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Reports.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetFinancialDashboardQueryHandler
        : IQueryHandler<
            GetFinancialDashboardQuery,
            ApiResponse<FinancialDashboardDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetFinancialDashboardQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<FinancialDashboardDto>> Handle(
            GetFinancialDashboardQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var tenantId =
                _currentTenant.TenantId;

            var salesRevenue =
                await connection.ExecuteScalarAsync<decimal>(
                    @"SELECT ISNULL(SUM(TotalAmount),0)
                      FROM Sales
                      WHERE TenantId=@TenantId
                      AND IsDeleted=0",
                    new { TenantId = tenantId });

            var purchaseCost =
                await connection.ExecuteScalarAsync<decimal>(
                    @"SELECT ISNULL(SUM(TotalAmount),0)
                      FROM Purchases
                      WHERE TenantId=@TenantId
                      AND IsDeleted=0",
                    new { TenantId = tenantId });

            var profit =
                await connection.ExecuteScalarAsync<decimal>(
                    @"SELECT ISNULL(SUM(ProfitAmount),0)
                      FROM Sales
                      WHERE TenantId=@TenantId
                      AND IsDeleted=0",
                    new { TenantId = tenantId });

            var cashInflow =
                await connection.ExecuteScalarAsync<decimal>(
                    @"SELECT ISNULL(SUM(Amount),0)
                      FROM CashTransactions
                      WHERE TenantId=@TenantId
                      AND IsDeleted=0
                      AND TransactionType IN (1,3)",
                    new { TenantId = tenantId });

            var cashOutflow =
                await connection.ExecuteScalarAsync<decimal>(
                    @"SELECT ISNULL(SUM(Amount),0)
                      FROM CashTransactions
                      WHERE TenantId=@TenantId
                      AND IsDeleted=0
                      AND TransactionType IN (2,4)",
                    new { TenantId = tenantId });

            var receivables =
                await connection.ExecuteScalarAsync<decimal>(
                    @"SELECT ISNULL(SUM(DueAmount),0)
                      FROM Sales
                      WHERE TenantId=@TenantId
                      AND IsDeleted=0",
                    new { TenantId = tenantId });

            var payables =
                await connection.ExecuteScalarAsync<decimal>(
                    @"SELECT ISNULL(SUM(DueAmount),0)
                      FROM Purchases
                      WHERE TenantId=@TenantId
                      AND IsDeleted=0",
                    new { TenantId = tenantId });

            var response =
                new FinancialDashboardDto
                {
                    TotalSalesRevenue = salesRevenue,

                    TotalPurchaseCost = purchaseCost,

                    TotalProfit = profit,

                    CashInflow = cashInflow,

                    CashOutflow = cashOutflow,

                    NetCashFlow =
                        cashInflow - cashOutflow,

                    TotalReceivables = receivables,

                    TotalPayables = payables
                };

            return ApiResponse<FinancialDashboardDto>
                .SuccessResponse(response);
        }
    }
}