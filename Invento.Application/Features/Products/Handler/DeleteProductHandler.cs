using Invento.Application.Common.Interface;
using Invento.Application.Data;
using Invento.Application.Features.Products.Commands;
using MediatR;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Products.Handler
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IDbConnectionFactory _db;
        private readonly ITenantProvider _tenant;

        public DeleteProductHandler(IDbConnectionFactory db, ITenantProvider tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var connection = _db.CreateConnection();

            var sql = @"DELETE FROM Products
                    WHERE Id = @Id AND TenantId = @TenantId";

            var affected = await connection.ExecuteAsync(sql, new
            {
                request.Id,
                TenantId = _tenant.GetTenantId()
            });

            if (affected == 0)
                throw new Exception("Delete failed or unauthorized");

            return Unit.Value;
        }
    }
}
