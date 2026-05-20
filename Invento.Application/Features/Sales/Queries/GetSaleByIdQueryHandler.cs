using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Sales.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Sales.Queries
{
    public class GetSaleByIdQueryHandler
        : IQueryHandler<GetSaleByIdQuery, ApiResponse<SaleDetailsDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetSaleByIdQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<SaleDetailsDto>> Handle(
            GetSaleByIdQuery request,
            CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            var saleSql = @"
            SELECT
                Id,
                InvoiceNumber,
                SaleDate,
                SubTotal,
                TaxAmount,
                DiscountAmount,
                TotalAmount,
                ProfitAmount
            FROM Sales
            WHERE Id = @Id
            AND IsDeleted = 0
            AND TenantId = @TenantId
            ";

            var itemsSql = @"
            SELECT
                si.ProductId,
                p.Name AS ProductName,
                si.Quantity,
                si.UnitPrice,
                si.TaxAmount,
                si.TotalPrice,
                si.ProfitAmount
            FROM SaleItems si
            INNER JOIN Products p
                ON si.ProductId = p.Id
            WHERE si.SaleId = @Id
            AND si.TenantId = @TenantId
            ";

            var sale = await connection.QueryFirstOrDefaultAsync<SaleDetailsDto>(
                    saleSql,
                    new
                    {
                        request.Id,
                        TenatId = _currentTenant.TenantId
                    }
            );

            if (sale is null)
            {
                return ApiResponse<SaleDetailsDto>
                    .FailureResponse(
                        new List<string>
                        {
                        "Sale not found"
                        }
                    );
            }

            var items = await connection.QueryAsync<SaleItemDto>(
                    itemsSql,
                    new
                    {
                        request.Id,
                        TenantId = _currentTenant.TenantId
                    }
            );

            sale.Items = items.ToList();

            return ApiResponse<SaleDetailsDto>
                .SuccessResponse(sale);
        }
    }
}