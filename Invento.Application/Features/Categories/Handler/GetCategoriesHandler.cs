using Invento.Application.Common.Interface;
using Invento.Application.Features.Categories.Queries;
using MediatR;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Categories.Handler
{
    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, IEnumerable<CategoryDto>>
    {
        private readonly IDbConnectionFactory _db;
        private readonly ITenantProvider _tenant;

        public GetCategoriesHandler(IDbConnectionFactory db, ITenantProvider tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<IEnumerable<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var connection = _db.CreateConnection();

            var sql = @"SELECT Id, Name
                    FROM Categories
                    WHERE TenantId = @TenantId";

            return await connection.QueryAsync<CategoryDto>(sql, new
            {
                TenantId = _tenant.GetTenantId()
            });
        }
    }
}
