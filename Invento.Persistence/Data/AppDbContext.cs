using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Invento.Shared.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Invento.Persistence.Data
{
    public class AppDbContext
        : DbContext, IApplicationDbContext
    {
        private readonly ICurrentUserService _currentUser;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            ICurrentUserService currentUser)
            : base(options)
        {
            _currentUser = currentUser;
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

        public DbSet<CashTransaction> CashTransactions => Set<CashTransaction>();

        public DbSet<CustomerPayment> CustomerPayments => Set<CustomerPayment>();

        public DbSet<SupplierPayment> SupplierPayments => Set<SupplierPayment>();

        public override async Task<int> SaveChangesAsync(
          CancellationToken cancellationToken = default)
        {
            var entries =
                ChangeTracker
                .Entries<AuditableEntity>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:

                        entry.Entity.CreatedAt =
                            DateTime.UtcNow;

                        entry.Entity.CreatedBy =
                            _currentUser.UserId ?? "System";

                        break;

                    case EntityState.Modified:

                        entry.Entity.UpdatedAt =
                            DateTime.UtcNow;

                        entry.Entity.UpdatedBy =
                            _currentUser.UserId ?? "System";
                        
                        break;
                }
            }

            return await base.SaveChangesAsync(
                cancellationToken);
        }

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

            modelBuilder.Entity<Purchase>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<CashTransaction>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<CustomerPayment>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<SupplierPayment>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Customer>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<User>()
                .HasQueryFilter(x => !x.IsDeleted);
        }

        public async Task<IDbContextTransaction>BeginTransactionAsync()
        {
            return await Database.BeginTransactionAsync();
        }
    }
}
