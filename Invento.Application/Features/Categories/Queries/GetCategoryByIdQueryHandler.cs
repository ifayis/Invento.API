using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Categories.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Categories.Queries;

public class GetCategoryByIdQueryHandler
    : IQueryHandler<
        GetCategoryByIdQuery,
        ApiResponse<CategoryDto>>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ICurrentTenantService _currentTenant;

    public GetCategoryByIdQueryHandler(
        IDbConnectionFactory connectionFactory,
        ICurrentTenantService currentTenant)
    {
        _connectionFactory = connectionFactory;
        _currentTenant = currentTenant;
    }

    public async Task<ApiResponse<CategoryDto>> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var sql = @"
        SELECT
            Id,
            Name,
            CreatedAt
        FROM Categories
        WHERE Id = @Id
        AND IsDeleted = 0
        AND TenantId = @TenantId
        ";

        var category = await connection
            .QueryFirstOrDefaultAsync<CategoryDto>(
                sql,
                new { request.Id,
                TenantId = _currentTenant.TenantId});

        if (category is null)
        {
            return ApiResponse<CategoryDto>
                .FailureResponse(
                    new List<string>
                    {
                        "Category not found"
                    });
        }

        return ApiResponse<CategoryDto>
            .SuccessResponse(category);
    }
}