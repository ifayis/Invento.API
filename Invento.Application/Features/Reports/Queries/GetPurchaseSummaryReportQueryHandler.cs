using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Reports.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Reports.Queries
{
    public class GetPurchaseSummaryReportQueryHandler
        : IQueryHandler<
            GetPurchaseSummaryReportQuery,
            ApiResponse<PurchaseSummaryReportDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetPurchaseSummaryReportQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<PurchaseSummaryReportDto>> Handle(
            GetPurchaseSummaryReportQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var sql = @"
            SELECT
                ISNULL(SUM(TotalAmount),0) AS TotalPurchases,
                COUNT(*) AS TotalPurchaseOrders,
                ISNULL(AVG(TotalAmount),0) AS AveragePurchaseValue
            FROM Purchases
            WHERE
                TenantId = @TenantId
                AND IsDeleted = 0
                AND
                (
                    @FromDate IS NULL
                    OR PurchaseDate >= @FromDate
                )
                AND
                (
                    @ToDate IS NULL
                    OR PurchaseDate <= @ToDate
                );
            ";

            Console.WriteLine("purchase Summary Handler Executed");

            Console.WriteLine(
                $"FromDate = {request.FromDate}");
            Console.WriteLine(
                $"ToDate = {request.ToDate}");
            Console.WriteLine(
                $"Tenant = {_currentTenant.TenantId}");


            var result =
                await connection.QueryFirstAsync<PurchaseSummaryReportDto>(
                    sql,
                    new
                    {
                        TenantId = _currentTenant.TenantId,
                        request.FromDate,
                        request.ToDate
                    });

            return ApiResponse<PurchaseSummaryReportDto>
                .SuccessResponse(result);
        }
    }
}