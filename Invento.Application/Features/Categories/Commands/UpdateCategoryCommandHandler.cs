using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Categories.Commands;

public class UpdateCategoryCommandHandler
    : ICommandHandler<
        UpdateCategoryCommand,
        ApiResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public UpdateCategoryCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Guid>> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(x =>
                x.Id == request.Id
                && !x.IsDeleted,
                cancellationToken);

        if (category is null)
        {
            return ApiResponse<Guid>
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
                && !x.IsDeleted,
                cancellationToken);

        if (exists)
        {
            return ApiResponse<Guid>
                .FailureResponse(
                    new List<string>
                    {
                        "Category name already exists"
                    });
        }

        category.Name = request.Name;

        await _context.SaveChangesAsync(
            cancellationToken);

        return ApiResponse<Guid>
            .SuccessResponse(
                category.Id,
                "Category updated successfully");
    }
}