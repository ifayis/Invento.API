using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Categories.Commands;

public class CreateCategoryCommandHandler
    : ICommandHandler<
        CreateCategoryCommand,
        ApiResponse<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateCategoryCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Guid>> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var exists = await _context.Categories
            .AnyAsync(x =>
                x.Name == request.Name
                && !x.IsDeleted,
                cancellationToken);

        if (exists)
        {
            return ApiResponse<Guid>
                .FailureResponse(
                    new List<string>
                    {
                        "Category already exists"
                    });
        }

        var category = new Category
        {
            Name = request.Name
        };

        await _context.Categories.AddAsync(
            category,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        return ApiResponse<Guid>
            .SuccessResponse(
                category.Id,
                "Category created successfully");
    }
}