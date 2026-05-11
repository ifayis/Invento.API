using Dapper;
using Invento.Application.Common.Interface;
using Invento.Domain.Entities;
using MediatR;

namespace Invento.Application.Features.Products.Queries
{
    public class GetProductsHandler
        : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IDbConnectionFactory _db;
        private readonly ITenantProvider _tenantProvider;

        public GetProductsHandler(
            IDbConnectionFactory db,
            ITenantProvider tenantProvider)
        {
            _db = db;
            _tenantProvider = tenantProvider;
        }

        public async Task<IEnumerable<ProductDto>> Handle(
            GetProductsQuery request,
            CancellationToken cancellationToken)
        {
            using var connection = _db.CreateConnection();

            var tenantId = _tenantProvider.GetTenantId();

            string sql = @"
                SELECT 
                    p.Id,
                    p.Name,
                    p.TagNumber,
                    p.StockQuantity,
                    p.SellingPrice,
                    c.Name AS CategoryName
                FROM Products p
                INNER JOIN Categories c
                    ON p.CategoryId = c.Id
                WHERE p.TenantId = @TenantId
            ";

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                sql += @"
                    AND (
                        p.Name LIKE @Search
                        OR p.TagNumber LIKE @Search
                    )
                ";
            }

            sql += " ORDER BY p.CreatedAt DESC";

            var products = await connection.QueryAsync<ProductDto>(
                sql,
                new
                {
                    TenantId = tenantId,
                    Search = $"%{request.Search}%"
                });

            return products;
        }
    }
}