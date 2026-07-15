using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Extensions;
using Invento.Application.Common.Caching;
using Invento.Application.Features.Categories.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Categories.Commands
{
    public class CreateCategoryCommandHandler
        : ICommandHandler<CreateCategoryCommand, ApiResponse<CategoryDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly ICacheVersionService _cacheVersionService;

        public CreateCategoryCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            ICacheVersionService cacheVersionService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _cacheVersionService = cacheVersionService;
        }

        public async Task<ApiResponse<CategoryDto>> Handle(
            CreateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentTenant.TenantId;

            var exists = await _context.Categories
                .AnyAsync(x =>
                    x.Name == request.Name
                    && x.TenantId == _currentTenant.TenantId
                    && !x.IsDeleted,
                    cancellationToken
                );

            if (exists)
            {
                return ApiResponse<CategoryDto>
                    .FailureResponse(
                        new List<string>
                        {
                        "Category already exists"
                        }
                    );
            }

            var category = new Category
            {
                TenantId = _currentTenant.TenantId,
                Name = request.Name
            };

            await _context.Categories.AddAsync(
                category,
                cancellationToken
            );

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
                    "Category created successfully"
                );
        }
    }
}