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

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
