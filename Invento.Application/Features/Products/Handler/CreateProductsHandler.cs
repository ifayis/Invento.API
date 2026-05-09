using Invento.Application.Common.Interface;
using MediatR;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Invento.Application.Features.Products.Commands;

namespace Invento.Application.Features.Products.Handler
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IDbConnectionFactory _db;
        private readonly ITenantProvider _tenant;

        public CreateProductHandler(IDbConnectionFactory db, ITenantProvider tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var connection = _db.CreateConnection();

            var productId = Guid.NewGuid();

            var sql = @"
INSERT INTO Products
(Id, TenantId, CategoryId, Name, TagNumber, Description,
 CostPrice, SellingPrice, TaxRate, StockQuantity,
 CreatedByUserId, CreatedAt)
VALUES
(@Id, @TenantId, @CategoryId, @Name, @TagNumber, @Description,
 @CostPrice, @SellingPrice, @TaxRate, @StockQuantity,
 @CreatedByUserId, GETUTCDATE())";

            await connection.ExecuteAsync(sql, new
            {
                Id = productId,
                TenantId = _tenant.GetTenantId(),
                request.CategoryId,
                request.Name,
                request.TagNumber,
                request.Description,
                request.CostPrice,
                request.SellingPrice,
                request.TaxRate,
                request.StockQuantity,
                CreatedByUserId = _tenant.GetUserId()
            });

            return productId;
        }
    }
}
