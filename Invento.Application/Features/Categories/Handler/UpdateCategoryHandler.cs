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
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly IDbConnectionFactory _db;
        private readonly ITenantProvider _tenant;

        public UpdateCategoryHandler(IDbConnectionFactory db, ITenantProvider tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var connection = _db.CreateConnection();

            var sql = @"UPDATE Categories
                    SET Name = @Name
                    WHERE Id = @Id AND TenantId = @TenantId";

            var affected = await connection.ExecuteAsync(sql, new
            {
                request.Id,
                request.Name,
                TenantId = _tenant.GetTenantId()
            });

            if (affected == 0)
                throw new Exception("Update failed");

            return Unit.Value;
        }
    }
}
