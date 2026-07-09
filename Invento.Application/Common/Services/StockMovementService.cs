using Invento.Application.Interfaces;
using Invento.Domain.Entities;

namespace Invento.Application.Common.Services
{
    public class StockMovementService
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;

        public StockMovementService(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant)
        {
            _context = context;
            _currentTenant = currentTenant;
        }

        public async Task CreateMovement(
            Guid productId,
            int quantity,
            string movementType,
            int currentStockAfterMovement,
            string? remarks,
            string? referenceNumber,
            CancellationToken cancellationToken)
        {
            var movement = new StockMovement
            {
                TenantId = _currentTenant.TenantId,
                ProductId = productId,
                Quantity = quantity,
                MovementType = movementType,
                CurrentStockAfterMovement =
                    currentStockAfterMovement,
                Remarks = remarks,
                ReferenceNumber = referenceNumber,
                CreatedAt = DateTime.UtcNow
            };

            await _context.StockMovements.AddAsync(
                movement,
                cancellationToken);
        }
    }
}