using Invento.Application.Common.Interface;
using Invento.Application.Data;
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
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IDbConnectionFactory _db;
        private readonly ITenantProvider _tenant;

        public GetProductByIdHandler(IDbConnectionFactory db, ITenantProvider tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var connection = _db.CreateConnection();

            var sql = @"SELECT Id, Name, SellingPrice, StockQuantity
                    FROM Products
                    WHERE Id = @Id AND TenantId = @TenantId";

            var product = await connection.QueryFirstOrDefaultAsync<ProductDto>(sql, new
            {
                request.Id,
                TenantId = _tenant.GetTenantId()
            });

            if (product == null)
                throw new Exception("Product not found");

            return product;
        }
    }
}
