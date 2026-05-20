using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Products.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Products.Queries
{
    public class GetProductByIdQueryHandler
        : IQueryHandler<GetProductByIdQuery, ApiResponse<ProductDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetProductByIdQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<ProductDto>> Handle(
            GetProductByIdQuery request,
            CancellationToken cancellationToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
            SELECT
                p.Id,
                p.Name,
                p.SKU,
                p.CostPrice,
                p.SellingPrice,
                p.CurrentStock,
                c.Name AS CategoryName,
                p.CreatedAt
            FROM Products p
            INNER JOIN Categories c
                ON p.CategoryId = c.Id
            WHERE p.Id = @Id
            AND p.IsDeleted = 0
            AND p.TenantId = @TenantId
            ";

            var product = await connection
                .QueryFirstOrDefaultAsync<ProductDto>(
                    sql,
                    new { request.Id,
                    TenantId = _currentTenant.TenantId}
                );

            if (product is null)
            {
                return ApiResponse<ProductDto>
                    .FailureResponse(
                        new List<string>
                        {
                        "Product not found"
                        }
                    );
            }

            return ApiResponse<ProductDto>
                .SuccessResponse(product);
        }
    }
}