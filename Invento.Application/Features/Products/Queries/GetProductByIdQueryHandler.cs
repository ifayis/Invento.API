using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Products.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Products.Queries
{
    public class GetProductByIdQueryHandler
        : IQueryHandler<
            GetProductByIdQuery,
            ApiResponse<ProductDto>>
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
            using var connection =
                _connectionFactory.CreateConnection();

            const string sql = """
                SELECT
                    p.Id,
                    p.CategoryId,
                    p.Name,
                    p.SKU,
                    p.CostPrice,
                    p.SellingPrice,
                    p.CurrentStock,
                    p.IsDeleted,
                    c.Name AS CategoryName,
                    p.CreatedAt,
                    p.LowStockThreshold,
                    p.CriticalStockThreshold
                FROM Products p
                INNER JOIN Categories c
                    ON c.Id = p.CategoryId
                    AND c.TenantId = @TenantId
                    AND c.IsDeleted = 0
                WHERE
                    p.Id = @Id
                    AND p.TenantId = @TenantId
                    AND p.IsDeleted = 0;
                """;

            var command =
                new CommandDefinition(
                    sql,
                    new
                    {
                        request.Id,
                        TenantId = _currentTenant.TenantId
                    },
                    cancellationToken:
                        cancellationToken);

            var product =
                await connection
                    .QueryFirstOrDefaultAsync<ProductDto>(
                        command);

            if (product is null)
            {
                return ApiResponse<ProductDto>
                    .FailureResponse(
                        new List<string>
                        {
                            "Product not found"
                        });
            }

            return ApiResponse<ProductDto>
                .SuccessResponse(product);
        }
    }
}