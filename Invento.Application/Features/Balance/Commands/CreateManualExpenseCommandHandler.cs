using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Caching;
using Invento.Application.Common.Extensions;
using Invento.Application.Features.Balance.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Invento.Domain.Enums;

namespace Invento.Application.Features.Balance.Commands
{
    public class CreateManualExpenseCommandHandler
        : ICommandHandler<
            CreateManualExpenseCommand,
            ApiResponse<CashTransactionDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly ICacheVersionService _cacheVersionService;

        public CreateManualExpenseCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            ICacheVersionService cacheVersionService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _cacheVersionService = cacheVersionService;
        }

        public async Task<ApiResponse<CashTransactionDto>> Handle(
            CreateManualExpenseCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId = _currentTenant.TenantId;

            var transaction = new CashTransaction
            {
                TenantId = _currentTenant.TenantId,
                TransactionType = CashTransactionType.ManualExpense,
                Amount = request.Amount,
                Description = request.Description.Trim(),
                TransactionDate = request.TransactionDate
            };

            await _context.CashTransactions.AddAsync(
                transaction,
                cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);

            await _cacheVersionService.InvalidateAsync(
                    tenantId,
                    CacheGroups.Balance,
                    CacheGroups.Reports,
                    CacheGroups.Dashboard);

            return ApiResponse<CashTransactionDto>
                .SuccessResponse(
                    new CashTransactionDto
                    {
                        Id = transaction.Id,
                        TransactionType = transaction.TransactionType,
                        Amount = transaction.Amount,
                        Description = transaction.Description,
                        TransactionDate = transaction.TransactionDate
                    },
                    "Expense added successfully"
                );
        }
    }
}