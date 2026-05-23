using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Categories.DTOs;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Categories.Commands;

public class RestoreCategoryCommandHandler
    : ICommandHandler<
        RestoreCategoryCommand,
        ApiResponse<CategoryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentTenantService _currentTenant;

    public RestoreCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentTenantService currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<ApiResponse<CategoryDto>> Handle(
        RestoreCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x =>
                x.Id == request.Id
                && x.TenantId ==
                    _currentTenant.TenantId,
                cancellationToken);

        if (category is null)
        {
            return ApiResponse<CategoryDto>
                .FailureResponse(
                    new List<string>
                    {
                        "Category not found"
                    });
        }

        if (!category.IsDeleted)
        {
            return ApiResponse<CategoryDto>
                .FailureResponse(
                    new List<string>
                    {
                        "Category already active"
                    });
        }

        category.IsDeleted = false;

        await _context.SaveChangesAsync(
            cancellationToken);

        return ApiResponse<CategoryDto>
            .SuccessResponse(
                new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name
                },
                "Category restored successfully");
    }
}