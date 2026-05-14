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

    public GetCategoryByIdQueryHandler(
        IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
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
        ";

        var category = await connection
            .QueryFirstOrDefaultAsync<CategoryDto>(
                sql,
                new { request.Id });

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