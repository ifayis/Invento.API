using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Categories.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Categories.Commands;

public class UpdateCategoryCommandHandler
    : ICommandHandler<
        UpdateCategoryCommand,
        ApiResponse<CategoryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentTenantService _currentTenant;

    public UpdateCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentTenantService currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<ApiResponse<CategoryDto>> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(x =>
                x.Id == request.Id
                && x.TenantId == _currentTenant.TenantId
                && !x.IsDeleted,
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

        var exists = await _context.Categories
            .AnyAsync(x =>
                x.Name == request.Name
                && x.Id != request.Id
                && x.TenantId == _currentTenant.TenantId
                && !x.IsDeleted,
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

        category.Name = request.Name;
        category.IsDeleted = request.IsDeleted;

        await _context.SaveChangesAsync(
            cancellationToken);

        return ApiResponse<CategoryDto>
            .SuccessResponse(
                new CategoryDto
                {
                   Name = category.Name
                },
                "Category updated successfully");
    }
}