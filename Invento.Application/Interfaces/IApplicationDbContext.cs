using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Invento.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }

        DbSet<Tenant> Tenants { get; }

        DbSet<RefreshToken> RefreshTokens { get; }

        DbSet<PasswordResetToken> PasswordResetTokens { get; }

        DbSet<Product> Products { get; }

        DbSet<Category> Categories { get; }

        DbSet<StockMovement> StockMovements { get; }

        DbSet<Sale> Sales { get; }

        DbSet<SaleItem> SaleItems { get; }

        DbSet<Customer> Customers { get; }

        DbSet<TenantSettings> TenantSettings { get; }

        DbSet<Supplier> Suppliers { get; }

        DbSet<Purchase> Purchases { get; }

        DbSet<PurchaseItem> PurchaseItems { get; }

        DbSet<CashTransaction> CashTransactions { get; }

        DbSet<CustomerPayment> CustomerPayments { get; }

        DbSet<SupplierPayment> SupplierPayments { get; }

        DbSet<AuditLog> AuditLogs { get; }

        DbSet<DocumentNumberSequence> DocumentNumberSequences { get; }

        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken);

        void ClearChangeTracker();

        Task<IDbContextTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = default);

        IExecutionStrategy CreateExecutionStrategy();
    }
}