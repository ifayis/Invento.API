using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Purchases.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Purchases.Queries
{
    public class GetPurchaseByIdQueryHandler
        : IQueryHandler<
            GetPurchaseByIdQuery,
            ApiResponse<PurchaseDetailsDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetPurchaseByIdQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<PurchaseDetailsDto>> Handle(
            GetPurchaseByIdQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            var purchaseSql = @"
            SELECT
                p.Id,
                p.SupplierId,
                s.Name AS SupplierName,
                p.PurchaseNumber,
                p.PurchaseDate,
                p.SubTotal,
                p.TaxAmount,
                p.DiscountAmount,
                p.TotalAmount,
                p.IsDeleted
            FROM Purchases p
            INNER JOIN Suppliers s
                ON p.SupplierId = s.Id
            WHERE
                p.Id = @Id
                AND p.TenantId = @TenantId
            ";

            var purchase =
                await connection.QueryFirstOrDefaultAsync
                <PurchaseDetailsDto>(
                    purchaseSql,
                    new
                    {
                        request.Id,
                        TenantId = _currentTenant.TenantId
                    });

            if (purchase is null)
            {
                return ApiResponse<PurchaseDetailsDto>
                    .FailureResponse(
                        ["Purchase not found"]);
            }

            var itemSql = @"
            SELECT
                pi.ProductId,
                pr.Name AS ProductName,
                pi.Quantity,
                pi.UnitCost,
                pi.TaxAmount,
                pi.TotalPrice
            FROM PurchaseItems pi
            INNER JOIN Products pr
                ON pi.ProductId = pr.Id
            WHERE
                pi.PurchaseId = @PurchaseId
                AND pi.TenantId = @TenantId
            ";

            var items =
                await connection.QueryAsync<PurchaseItemDto>(
                    itemSql,
                    new
                    {
                        PurchaseId = purchase.Id,
                        TenantId = _currentTenant.TenantId
                    });

            purchase.Items = items.ToList();

            return ApiResponse<PurchaseDetailsDto>
                .SuccessResponse(purchase);
        }
    }
}