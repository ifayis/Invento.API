using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Categories.Commands;

public class DeleteCategoryCommandHandler
    : ICommandHandler<
        DeleteCategoryCommand,
        ApiResponse<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentTenantService _currentTenant;

    public DeleteCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentTenantService currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<ApiResponse<Guid>> Handle(
        DeleteCategoryCommand request,
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
            return ApiResponse<Guid>
                .FailureResponse(
                    new List<string>
                    {
                        "Category not found"
                    });
        }

        category.IsDeleted = true;

        await _context.SaveChangesAsync(
            cancellationToken);

        return ApiResponse<Guid>
            .SuccessResponse(
                category.Id,
                "Category deleted successfully");
    }
}