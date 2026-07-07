using Invento.Application.Common.Jobs;
using Invento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invento.Infrastructure.Jobs
{
    public class RecurringJobService
        : IRecurringJobService
    {
        private readonly IApplicationDbContext _context;

        private readonly ILogger<RecurringJobService> _logger;

        public RecurringJobService(
            IApplicationDbContext context,
            ILogger<RecurringJobService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ExecuteLowStockCheck()
        {
            var lowStockProducts =
                await _context.Products
                .Where(x =>
                    !x.IsDeleted
                    &&
                    x.CurrentStock <= x.LowStockThreshold)
                .Select(x => new
                {
                    x.Name,
                    x.CurrentStock,
                    x.LowStockThreshold,
                    x.CriticalStockThreshold
                })
                .ToListAsync();

            foreach (var product in lowStockProducts)
            {
                var level =
                    product.CurrentStock
                    <= product.CriticalStockThreshold
                        ? "CRITICAL"
                        : "LOW";

                _logger.LogWarning(
                    "Stock Alert | Product: {Product} | Current Stock: {CurrentStock} | Level: {Level}",
                    product.Name,
                    product.CurrentStock,
                    level);
            }
        }

        public async Task ExecuteSalesTargetCheck()
        {
            var currentMonth = DateTime.UtcNow.Month;

            var currentYear = DateTime.UtcNow.Year;

            var settings =
                await _context.TenantSettings
                .Where(x => !x.IsDeleted)
                .ToListAsync();

            foreach (var setting in settings)
            {
                var monthlySales =
                    await _context.Sales
                    .Where(x =>
                        x.TenantId == setting.TenantId
                        &&
                        !x.IsDeleted
                        &&
                        x.SaleDate.Month == currentMonth
                        &&
                        x.SaleDate.Year == currentYear)
                    .SumAsync(x =>
                        (decimal?)x.TotalAmount)
                    ?? 0;

                if (setting.MonthlySalesTarget <= 0)
                {
                    continue;
                }

                var achievement =
                    (monthlySales
                    / setting.MonthlySalesTarget)
                    * 100;

                _logger.LogInformation(
                    "Sales Target Check | Tenant: {TenantId} | Sales: {Sales} | Target: {Target} | Achievement: {Achievement}%",
                    setting.TenantId,
                    monthlySales,
                    setting.MonthlySalesTarget,
                    Math.Round(achievement, 2));
            }
        }
        public async Task ExecuteProfitTargetCheck()
        {
            var currentMonth = DateTime.UtcNow.Month;

            var currentYear = DateTime.UtcNow.Year;

            var settings =
                await _context.TenantSettings
                .Where(x => !x.IsDeleted)
                .ToListAsync();

            foreach (var setting in settings)
            {
                var monthlyProfit =
                    await _context.Sales
                    .Where(x =>
                        x.TenantId == setting.TenantId
                        &&
                        !x.IsDeleted
                        &&
                        x.SaleDate.Month == currentMonth
                        &&
                        x.SaleDate.Year == currentYear)
                    .SumAsync(x =>
                        (decimal?)x.ProfitAmount)
                    ?? 0;

                if (setting.MonthlyProfitTarget <= 0)
                {
                    continue;
                }

                var achievement =
                    (monthlyProfit
                    / setting.MonthlyProfitTarget)
                    * 100;

                _logger.LogInformation(
                    "Profit Target Check | Tenant: {TenantId} | Profit: {Profit} | Target: {Target} | Achievement: {Achievement}%",
                    setting.TenantId,
                    monthlyProfit,
                    setting.MonthlyProfitTarget,
                    Math.Round(achievement, 2));
            }
        }
        public async Task ExecuteReceivableCheck()
        {
            var receivables =
                await _context.Sales
                .Where(x =>
                    !x.IsDeleted
                    &&
                    x.DueAmount > 0)
                .Select(x => new
                {
                    x.InvoiceNumber,
                    x.CustomerId,
                    x.TotalAmount,
                    x.PaidAmount,
                    x.DueAmount,
                    x.PaymentStatus
                })
                .ToListAsync();

            if (!receivables.Any())
            {
                _logger.LogInformation(
                    "Receivable Check | No outstanding receivables found.");

                return;
            }

            var totalOutstanding =
                receivables.Sum(x => x.DueAmount);

            _logger.LogWarning(
                "Receivable Check | Outstanding Invoices: {Count} | Outstanding Amount: {Amount}",
                receivables.Count,
                totalOutstanding);

            foreach (var item in receivables)
            {
                _logger.LogWarning(
                    "Invoice: {InvoiceNumber} | Due Amount: {DueAmount} | Status: {Status}",
                    item.InvoiceNumber,
                    item.DueAmount,
                    item.PaymentStatus);
            }
        }

        public async Task ExecutePayableCheck()
        {
            var payables =
                await _context.Purchases
                .Where(x =>
                    !x.IsDeleted
                    &&
                    x.DueAmount > 0)
                .Select(x => new
                {
                    x.PurchaseNumber,
                    x.SupplierId,
                    x.TotalAmount,
                    x.PaidAmount,
                    x.DueAmount,
                    x.PaymentStatus
                })
                .ToListAsync();

            if (!payables.Any())
            {
                _logger.LogInformation(
                    "Payable Check | No outstanding payables found.");

                return;
            }

            var totalOutstanding =
                payables.Sum(x => x.DueAmount);

            _logger.LogWarning(
                "Payable Check | Outstanding Purchases: {Count} | Outstanding Amount: {Amount}",
                payables.Count,
                totalOutstanding);

            foreach (var item in payables)
            {
                _logger.LogWarning(
                    "Purchase: {PurchaseNumber} | Due Amount: {DueAmount} | Status: {Status}",
                    item.PurchaseNumber,
                    item.DueAmount,
                    item.PaymentStatus);
            }
        }

        public async Task ExecuteRefreshTokenCleanup()
        {
            var cutoffDate =
                DateTime.UtcNow.AddDays(-30);

            var tokensToDelete =
                await _context.RefreshTokens
                    .IgnoreQueryFilters()
                    .Where(x =>
                        x.ExpiresAt < cutoffDate
                        ||
                        (
                            x.IsRevoked
                            &&
                            x.RevokedAt.HasValue
                            &&
                            x.RevokedAt.Value < cutoffDate
                        ))
                    .ToListAsync();

            if (!tokensToDelete.Any())
            {
                _logger.LogInformation(
                    "Refresh Token Cleanup | No old tokens found.");

                return;
            }

            _context.RefreshTokens.RemoveRange(
                tokensToDelete);

            await _context.SaveChangesAsync(
                CancellationToken.None);

            _logger.LogInformation(
                "Refresh Token Cleanup | Deleted Tokens: {Count}",
                tokensToDelete.Count);
        }
    }
}