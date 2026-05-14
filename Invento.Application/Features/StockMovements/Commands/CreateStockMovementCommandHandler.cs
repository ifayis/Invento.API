using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Invento.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Invento.Application.Features.StockMovements.Commands
{
    public class CreateStockMovementCommandHandler
        : ICommandHandler<
            CreateStockMovementCommand,
            ApiResponse<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateStockMovementCommandHandler(
            IApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<Guid>> Handle(
            CreateStockMovementCommand request,
            CancellationToken cancellationToken)
        {
            using var transaction =
                await _context.BeginTransactionAsync();

            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(x =>
                        x.Id == request.ProductId
                        && !x.IsDeleted,
                        cancellationToken);

                if (product is null)
                {
                    return ApiResponse<Guid>
                        .FailureResponse(
                            new List<string>
                            {
                            "Product not found"
                            });
                }

                if (request.MovementType
                    == StockMovementType.StockOut)
                {
                    if (product.CurrentStock
                        < request.Quantity)
                    {
                        return ApiResponse<Guid>
                            .FailureResponse(
                                new List<string>
                                {
                                "Insufficient stock"
                                });
                    }

                    product.CurrentStock -= request.Quantity;
                }
                else
                {
                    product.CurrentStock += request.Quantity;
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
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    MovementType =
                        request.MovementType.ToString(),
                    Remarks = request.Remarks,
                    ReferenceNumber =
                        request.ReferenceNumber,
                    CreatedByUserId = userId
                };

                await _context.StockMovements.AddAsync(
                    movement,
                    cancellationToken);

                await _context.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);

                return ApiResponse<Guid>
                    .SuccessResponse(
                        movement.Id,
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