using Invento.Application.Common.Interface;
using Invento.Application.Features.Products.Queries;
using MediatR;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Products.Handler
{
    public class SearchProductsHandler : IRequestHandler<SearchProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IDbConnectionFactory _db;
        private readonly ITenantProvider _tenant;

        public SearchProductsHandler(IDbConnectionFactory db, ITenantProvider tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<IEnumerable<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
        {
            var connection = _db.CreateConnection();

            var sql = @"SELECT Id, Name, SellingPrice, StockQuantity
                    FROM Products
                    WHERE TenantId = @TenantId
                    AND Name LIKE '%' + @Search + '%'";

            return await connection.QueryAsync<ProductDto>(sql, new
            {
                TenantId = _tenant.GetTenantId(),
                request.Search
            });
        }
    }
}
