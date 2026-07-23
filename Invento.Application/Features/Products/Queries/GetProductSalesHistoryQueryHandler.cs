using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Products.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Products.Queries
{
    public class GetProductSalesHistoryQueryHandler
        : IQueryHandler<
            GetProductSalesHistoryQuery,
            ApiResponse<List<ProductSalesHistoryDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetProductSalesHistoryQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<ProductSalesHistoryDto>>> Handle(
            GetProductSalesHistoryQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            const string sql = @"
            SELECT
                s.Id AS SaleId,
                s.InvoiceNumber,
                s.SaleDate,
                si.Quantity,
                si.UnitPrice,
                si.TotalPrice,
                si.ProfitAmount,
                ISNULL(c.Name,'Walk-in Customer') AS CustomerName
            FROM SaleItems si
            INNER JOIN Sales s
                ON si.SaleId = s.Id
            LEFT JOIN Customers c
                ON s.CustomerId = c.Id
            WHERE
                si.ProductId = @ProductId
                AND si.TenantId = @TenantId
                AND s.IsDeleted = 0
            ORDER BY s.SaleDate DESC;
            ";

            var result =
                (await connection.QueryAsync<ProductSalesHistoryDto>(
                    sql,
                    new
                    {
                        request.ProductId,
                        TenantId = _currentTenant.TenantId
                    }))
                .ToList();

            return ApiResponse<List<ProductSalesHistoryDto>>
                .SuccessResponse(result);
        }
    }
}