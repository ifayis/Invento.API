using Invento.Application.Common.Interface;
using Invento.Application.Data;
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
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Guid>
    {
        private readonly IDbConnectionFactory _db;
        private readonly ITenantProvider _tenant;

        public CreateCategoryHandler(IDbConnectionFactory db, ITenantProvider tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var connection = _db.CreateConnection();

            var exists = await connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(1)
              FROM Categories
              WHERE Name = @Name AND TenantId = @TenantId",
                new
                {
                    request.Name,
                    TenantId = _tenant.GetTenantId()
                });

            if (exists > 0)
                throw new Exception("Category already exists");

            var id = Guid.NewGuid();

            var sql = @"INSERT INTO Categories
                    (Id, TenantId, Name, CreatedByUserId, CreatedAt)
                    VALUES
                    (@Id, @TenantId, @Name, @UserId, GETUTCDATE())";

            await connection.ExecuteAsync(sql, new
            {
                Id = id,
                request.Name,
                TenantId = _tenant.GetTenantId(),
                UserId = _tenant.GetUserId()
            });

            return id;
        }
    }
}
