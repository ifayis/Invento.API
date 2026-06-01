using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Invento.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Invento.Application.Features.StockMovements.DTOs;

namespace Invento.Application.Features.StockMovements.Commands
{
    public class CreateStockMovementCommandHandler
        : ICommandHandler< CreateStockMovementCommand, ApiResponse<StockMovementDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentTenantService _currentTenant;

        public CreateStockMovementCommandHandler(
            IApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ICurrentTenantService currentTenant)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<StockMovementDto>> Handle(
            CreateStockMovementCommand request,
            CancellationToken cancellationToken)
        {
            using var transaction = await _context.BeginTransactionAsync();

            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(x =>
                        x.Id == request.ProductId
                        && x.TenantId == _currentTenant.TenantId
                        && !x.IsDeleted,
                        cancellationToken
                    );

                if (product is null)
                {
                    return ApiResponse<StockMovementDto>
                        .FailureResponse(
                            new List<string>
                            {
                            "Product not found"
                            }
                        );
                }

                switch (request.MovementType)
                {
                    case StockMovementType.AdjustmentIn:

                        product.CurrentStock += request.Quantity;
                        break;

                    case StockMovementType.AdjustmentOut:

                        if (product.CurrentStock < request.Quantity)
                        {
                            return ApiResponse<StockMovementDto>
                                .FailureResponse(
                                [
                                    "Insufficient stock"
                                ]);
                        }

                        product.CurrentStock -= request.Quantity;

                        break;
                }
                Guid? userId = null;

                var userIdClaim =
                    _httpContextAccessor.HttpContext?
                    .User.FindFirst("Name")?.Value;

                if (!string.IsNullOrWhiteSpace(userIdClaim))
                {
                    userId = Guid.Parse(userIdClaim);
                }

                var movement = new StockMovement
                {
                    TenantId = _currentTenant.TenantId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    MovementType = request.MovementType.ToString(),
                    Remarks = request.Remarks,
                    ReferenceNumber = request.ReferenceNumber,
                    CreatedByUserId = userId
                };

                await _context.StockMovements.AddAsync(
                    movement,
                    cancellationToken
                );

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return ApiResponse<StockMovementDto>
                .SuccessResponse(
                new StockMovementDto
                {
                    Id = movement.Id,

                    ProductId = product.Id,

                    ProductName = product.Name,

                    Quantity = movement.Quantity,

                    MovementType = movement.MovementType,

                    CurrentStockAfterMovement =
                        product.CurrentStock,

                    Remarks = movement.Remarks,

                    ReferenceNumber =
                        movement.ReferenceNumber,
                },
                "Stock movement completed");
            }
            catch
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                throw;
            }
        }
    }
}