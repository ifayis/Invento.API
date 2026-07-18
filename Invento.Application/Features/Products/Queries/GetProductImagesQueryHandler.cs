using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Products.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Products.Queries
{
    public class GetProductImagesQueryHandler
        : IQueryHandler<
            GetProductImagesQuery,
            ApiResponse<List<ProductImageDto>>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetProductImagesQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<List<ProductImageDto>>> Handle(
            GetProductImagesQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            const string sql = """
                SELECT
                    Id,
                    ImageUrl,
                    OriginalFileName,
                    IsPrimary
                FROM ProductImages
                WHERE
                    ProductId = @ProductId
                    AND TenantId = @TenantId
                    AND IsDeleted = 0
                ORDER BY
                    IsPrimary DESC,
                    CreatedAt;
                """;

            var command =
                new CommandDefinition(
                    sql,
                    new
                    {
                        request.ProductId,
                        TenantId =
                            _currentTenant.TenantId
                    },
                    cancellationToken:
                        cancellationToken);

            var images =
                (await connection.QueryAsync<ProductImageDto>(
                    command))
                .ToList();

            return ApiResponse<List<ProductImageDto>>
                .SuccessResponse(images);
        }
    }
}