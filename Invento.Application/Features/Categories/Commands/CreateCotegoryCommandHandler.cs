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
    private readonly ICurrentTenantService _currentTenant;

    public CreateCategoryCommandHandler(
        IApplicationDbContext context, 
        ICurrentTenantService currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<ApiResponse<Guid>> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var exists = await _context.Categories
            .AnyAsync(x =>
                x.Name == request.Name
                && x.TenantId == _currentTenant.TenantId
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
            TenantId = _currentTenant.TenantId,
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