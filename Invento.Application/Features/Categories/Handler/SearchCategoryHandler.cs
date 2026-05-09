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
    public class SearchCategoryHandler : IRequestHandler<SearchCategoryQuery, IEnumerable<CategoryDto>>
    {
        private readonly IDbConnectionFactory _db;
        private readonly ITenantProvider _tenant;

        public SearchCategoryHandler(IDbConnectionFactory db, ITenantProvider tenant)
        {
            _db = db;
            _tenant = tenant;
        }

        public async Task<IEnumerable<CategoryDto>> Handle(SearchCategoryQuery request, CancellationToken cancellationToken)
        {
            var connection = _db.CreateConnection();

            var sql = @"SELECT Id, Name
                    FROM Categories
                    WHERE TenantId = @TenantId
                    AND Name LIKE '%' + @Search + '%'";

            return await connection.QueryAsync<CategoryDto>(sql, new
            {
                TenantId = _tenant.GetTenantId(),
                request.Search
            });
        }
    }
}
