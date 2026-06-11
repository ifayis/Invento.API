using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Invento.Persistence.Data
{
    public class AppDbContext
        : DbContext, IApplicationDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tenant> Tenants => Set<Tenant>();

        public DbSet<User> Users => Set<User>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<Category> Categories => Set<Category>();

        public DbSet<Product> Products => Set<Product>();

        public DbSet<ProductImage> ProductImages => Set<ProductImage>();

        public DbSet<StockMovement> StockMovements => Set<StockMovement>();

        public DbSet<Customer> Customers => Set<Customer>();

        public DbSet<Sale> Sales => Set<Sale>();

        public DbSet<SaleItem> SaleItems => Set<SaleItem>();

        public DbSet<TenantSettings> TenantSettings => Set<TenantSettings>();

        public DbSet<Supplier> Suppliers => Set<Supplier>();

        public DbSet<Purchase> Purchases => Set<Purchase>();

        public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AppDbContext).Assembly);

            modelBuilder.Entity<Product>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Category>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Sale>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<StockMovement>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Supplier>()
                .HasQueryFilter(x => !x.IsDeleted);
        }

        public async Task<IDbContextTransaction>BeginTransactionAsync()
        {
            return await Database.BeginTransactionAsync();
        }
    }
}
