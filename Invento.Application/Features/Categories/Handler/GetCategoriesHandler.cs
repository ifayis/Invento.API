using Dapper;
using Invento.Application.Common.Interface;
using MediatR;

namespace Invento.Application.Features.Categories.Queries
{
    public class GetCategoriesHandler
        : IRequestHandler<GetCategoriesQuery, IEnumerable<CategoryDto>>
    {
        private readonly IDbConnectionFactory _db;
        private readonly ITenantProvider _tenantProvider;

        public GetCategoriesHandler(
            IDbConnectionFactory db,
            ITenantProvider tenantProvider)
        {
            _db = db;
            _tenantProvider = tenantProvider;
        }

        public async Task<IEnumerable<CategoryDto>> Handle(
            GetCategoriesQuery request,
            CancellationToken cancellationToken)
        {
            using var connection = _db.CreateConnection();

            var tenantId = _tenantProvider.GetTenantId();

            string sql = @"
                SELECT 
                    Id,
                    Name,
                    CreatedAt
                FROM Categories
                WHERE TenantId = @TenantId
            ";

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                sql += " AND Name LIKE @Search";
            }

            sql += " ORDER BY CreatedAt DESC";

            var categories = await connection.QueryAsync<CategoryDto>(
                sql,
                new
                {
                    TenantId = tenantId,
                    Search = $"%{request.Search}%"
                });

            return categories;
        }
    }
}