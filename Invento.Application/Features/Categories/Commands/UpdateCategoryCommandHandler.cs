using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Common.Extensions;
using Invento.Application.Features.Categories.DTOs;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Categories.Commands;

public class UpdateCategoryCommandHandler
    : ICommandHandler<
        UpdateCategoryCommand,
        ApiResponse<CategoryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentTenantService _currentTenant;
    private readonly ICacheVersionService _cacheVersionService;

    public UpdateCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentTenantService currentTenant,
        ICacheVersionService cacheVersionService)
    {
        _context = context;
        _currentTenant = currentTenant;
        _cacheVersionService = cacheVersionService;
    }

    public async Task<ApiResponse<CategoryDto>> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = _currentTenant.TenantId;

        var category = await _context.Categories
            .FirstOrDefaultAsync(
                x =>
                    x.Id == request.Id &&
                    x.TenantId == tenantId &&
                    !x.IsDeleted,
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

        var categoryName = request.Name?.Trim();

        if (string.IsNullOrWhiteSpace(categoryName))
        {
            return ApiResponse<CategoryDto>
                .FailureResponse(
                    new List<string>
                    {
                    "Category name is required"
                    });
        }

        var exists = await _context.Categories
            .AnyAsync(
                x =>
                    x.Id != request.Id &&
                    x.TenantId == tenantId &&
                    !x.IsDeleted &&
                    x.Name == categoryName,
                cancellationToken);

        if (exists)
        {
            return ApiResponse<CategoryDto>
                .FailureResponse(
                    new List<string>
                    {
                    "Category name already exists"
                    });
        }

        category.Name = categoryName;

        await _context.SaveChangesAsync(cancellationToken);

        await _cacheVersionService.InvalidateAsync(
            tenantId,
            CacheGroups.Categories,
            CacheGroups.Products,
            CacheGroups.Reports,
            CacheGroups.Dashboard);

        return ApiResponse<CategoryDto>
            .SuccessResponse(
                new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name
                },
                "Category updated successfully");
    }
}