using Invento.Application.Common.Interface;
using Invento.Application.Features.Categories.Command;
using MediatR;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Categories.Handler
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly IDbConnectionFactory _db;
        private readonly ITenantProvider _tenant;

        public DeleteCategoryHandler(IDbConnectionFactory db, ITenantProvider tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var connection = _db.CreateConnection();

            var used = await connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(1)
              FROM Products
              WHERE CategoryId = @Id AND TenantId = @TenantId",
                new
                {
                    request.Id,
                    TenantId = _tenant.GetTenantId()
                });

            if (used > 0)
                throw new Exception("Cannot delete category in use");

            var sql = @"DELETE FROM Categories
                    WHERE Id = @Id AND TenantId = @TenantId";

            await connection.ExecuteAsync(sql, new
            {
                request.Id,
                TenantId = _tenant.GetTenantId()
            });

            return Unit.Value;
        }
    }
}
