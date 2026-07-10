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
                    .AsNoTracking()
                    .Where(x =>
                        !x.IsDeleted
                        &&
                        x.CurrentStock <= x.LowStockThreshold)
                    .Select(x => new
                    {
                        x.TenantId,
                        x.Name,
                        x.CurrentStock,
                        x.LowStockThreshold,
                        x.CriticalStockThreshold
                    })
                    .ToListAsync();

            if (lowStockProducts.Count == 0)
            {
                _logger.LogInformation(
                    "Stock Alert Check | No low-stock products found.");

                return;
            }

            foreach (var product in lowStockProducts)
            {
                var level =
                    product.CurrentStock
                    <= product.CriticalStockThreshold
                        ? "CRITICAL"
                        : "LOW";

                _logger.LogWarning(
                    "Stock Alert | Tenant: {TenantId} | Product: {Product} | Current Stock: {CurrentStock} | Level: {Level}",
                    product.TenantId,
                    product.Name,
                    product.CurrentStock,
                    level);
            }
        }

        public async Task ExecuteSalesTargetCheck()
        {
            var utcNow = DateTime.UtcNow;

            var monthStart =
                new DateTime(
                    utcNow.Year,
                    utcNow.Month,
                    1,
                    0,
                    0,
                    0,
                    DateTimeKind.Utc);

            var nextMonthStart =
                monthStart.AddMonths(1);

            var settings =
                await _context.TenantSettings
                    .AsNoTracking()
                    .Where(x =>
                        !x.IsDeleted
                        &&
                        x.MonthlySalesTarget > 0)
                    .Select(x => new
                    {
                        x.TenantId,
                        x.MonthlySalesTarget
                    })
                    .ToListAsync();

            if (settings.Count == 0)
            {
                _logger.LogInformation(
                    "Sales Target Check | No active sales targets found.");

                return;
            }

            var tenantIds =
                settings
                    .Select(x => x.TenantId)
                    .Distinct()
                    .ToList();

            var salesByTenant =
                await _context.Sales
                    .AsNoTracking()
                    .Where(x =>
                        tenantIds.Contains(x.TenantId)
                        &&
                        !x.IsDeleted
                        &&
                        x.SaleDate >= monthStart
                        &&
                        x.SaleDate < nextMonthStart)
                    .GroupBy(x => x.TenantId)
                    .Select(group => new
                    {
                        TenantId = group.Key,

                        TotalSales =
                            group.Sum(x => x.TotalAmount)
                    })
                    .ToDictionaryAsync(
                        x => x.TenantId,
                        x => x.TotalSales);

            foreach (var setting in settings)
            {
                var monthlySales =
                    salesByTenant.TryGetValue(
                        setting.TenantId,
                        out var totalSales)
                            ? totalSales
                            : 0m;

                var achievement =
                    monthlySales
                    / setting.MonthlySalesTarget
                    * 100m;

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
            var utcNow = DateTime.UtcNow;

            var monthStart =
                new DateTime(
                    utcNow.Year,
                    utcNow.Month,
                    1,
                    0,
                    0,
                    0,
                    DateTimeKind.Utc);

            var nextMonthStart =
                monthStart.AddMonths(1);

            var settings =
                await _context.TenantSettings
                    .AsNoTracking()
                    .Where(x =>
                        !x.IsDeleted
                        &&
                        x.MonthlyProfitTarget > 0)
                    .Select(x => new
                    {
                        x.TenantId,
                        x.MonthlyProfitTarget
                    })
                    .ToListAsync();

            if (settings.Count == 0)
            {
                _logger.LogInformation(
                    "Profit Target Check | No active profit targets found.");

                return;
            }

            var tenantIds =
                settings
                    .Select(x => x.TenantId)
                    .Distinct()
                    .ToList();

            var profitByTenant =
                await _context.Sales
                    .AsNoTracking()
                    .Where(x =>
                        tenantIds.Contains(x.TenantId)
                        &&
                        !x.IsDeleted
                        &&
                        x.SaleDate >= monthStart
                        &&
                        x.SaleDate < nextMonthStart)
                    .GroupBy(x => x.TenantId)
                    .Select(group => new
                    {
                        TenantId = group.Key,

                        TotalProfit =
                            group.Sum(x => x.ProfitAmount)
                    })
                    .ToDictionaryAsync(
                        x => x.TenantId,
                        x => x.TotalProfit);

            foreach (var setting in settings)
            {
                var monthlyProfit =
                    profitByTenant.TryGetValue(
                        setting.TenantId,
                        out var totalProfit)
                            ? totalProfit
                            : 0m;

                var achievement =
                    monthlyProfit
                    / setting.MonthlyProfitTarget
                    * 100m;

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
                    .AsNoTracking()
                    .Where(x =>
                        !x.IsDeleted
                        &&
                        x.DueAmount > 0)
                    .Select(x => new
                    {
                        x.TenantId,
                        x.InvoiceNumber,
                        x.CustomerId,
                        x.DueAmount,
                        x.PaymentStatus
                    })
                    .ToListAsync();

            if (receivables.Count == 0)
            {
                _logger.LogInformation(
                    "Receivable Check | No outstanding receivables found.");

                return;
            }

            var summaries =
                receivables
                    .GroupBy(x => x.TenantId)
                    .Select(group => new
                    {
                        TenantId = group.Key,
                        Count = group.Count(),
                        TotalOutstanding =
                            group.Sum(x => x.DueAmount)
                    });

            foreach (var summary in summaries)
            {
                _logger.LogWarning(
                    "Receivable Check | Tenant: {TenantId} | Outstanding Invoices: {Count} | Outstanding Amount: {Amount}",
                    summary.TenantId,
                    summary.Count,
                    summary.TotalOutstanding);
            }

            foreach (var item in receivables)
            {
                _logger.LogWarning(
                    "Receivable | Tenant: {TenantId} | Invoice: {InvoiceNumber} | Due Amount: {DueAmount} | Status: {Status}",
                    item.TenantId,
                    item.InvoiceNumber,
                    item.DueAmount,
                    item.PaymentStatus);
            }
        }

        public async Task ExecutePayableCheck()
        {
            var payables =
                await _context.Purchases
                    .AsNoTracking()
                    .Where(x =>
                        !x.IsDeleted
                        &&
                        x.DueAmount > 0)
                    .Select(x => new
                    {
                        x.TenantId,
                        x.PurchaseNumber,
                        x.SupplierId,
                        x.DueAmount,
                        x.PaymentStatus
                    })
                    .ToListAsync();

            if (payables.Count == 0)
            {
                _logger.LogInformation(
                    "Payable Check | No outstanding payables found.");

                return;
            }

            var summaries =
                payables
                    .GroupBy(x => x.TenantId)
                    .Select(group => new
                    {
                        TenantId = group.Key,
                        Count = group.Count(),
                        TotalOutstanding =
                            group.Sum(x => x.DueAmount)
                    });

            foreach (var summary in summaries)
            {
                _logger.LogWarning(
                    "Payable Check | Tenant: {TenantId} | Outstanding Purchases: {Count} | Outstanding Amount: {Amount}",
                    summary.TenantId,
                    summary.Count,
                    summary.TotalOutstanding);
            }

            foreach (var item in payables)
            {
                _logger.LogWarning(
                    "Payable | Tenant: {TenantId} | Purchase: {PurchaseNumber} | Due Amount: {DueAmount} | Status: {Status}",
                    item.TenantId,
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

            if (tokensToDelete.Count == 0)
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