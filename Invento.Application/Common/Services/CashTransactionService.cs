using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Invento.Domain.Enums;

namespace Invento.Application.Common.Services
{
    public class CashTransactionService
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;

        public CashTransactionService(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant)
        {
            _context = context;
            _currentTenant = currentTenant;
        }

        public async Task<CashTransaction> CreateTransaction(
            CashTransactionType type,
            decimal amount,
            string description,
            DateTime transactionDate)
        {
            var transaction = new CashTransaction
            {
                TenantId = _currentTenant.TenantId,

                TransactionType = type,

                Amount = amount,

                Description = description,

                TransactionDate = transactionDate
            };

            await _context.CashTransactions
                .AddAsync(transaction);

            return transaction;
        }
    }
}