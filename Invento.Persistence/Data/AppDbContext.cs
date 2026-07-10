using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Invento.Persistence.Auditing;
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

        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

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

        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        public DbSet<DocumentNumberSequence> DocumentNumberSequences => Set<DocumentNumberSequence>();

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();

            var auditLogs = new List<AuditLog>();

            var auditableEntries = ChangeTracker
                .Entries<AuditableEntity>()
                .Where(x =>
                    x.State == EntityState.Added ||
                    x.State == EntityState.Modified ||
                    x.State == EntityState.Deleted)
                .ToList();

            var saleItemEntries = ChangeTracker
                .Entries<SaleItem>()
                .Where(x =>
                    x.State == EntityState.Added ||
                    x.State == EntityState.Deleted)
                .ToList();

            var purchaseItemEntries = ChangeTracker
                .Entries<PurchaseItem>()
                .Where(x =>
                    x.State == EntityState.Added ||
                    x.State == EntityState.Deleted)
                .ToList();

            var now = DateTime.UtcNow;

            foreach (var entry in auditableEntries)
            {
                var entity = entry.Entity;

                switch (entry.State)
                {
                    case EntityState.Added:
                        {
                            entity.CreatedAt = now;

                            entity.CreatedBy =
                                _currentUser.UserId;

                            auditLogs.Add(
                                new AuditLog
                                {
                                    TenantId = entity.TenantId,

                                    UserId =
                                        _currentUser.UserId,

                                    EntityName =
                                        entry.Metadata.ClrType.Name,

                                    ActionType = "Create",

                                    RecordId =
                                        entity.Id.ToString(),

                                    OldValues = null,

                                    NewValues =
                                        AuditValueSerializer
                                            .GetCreatedValues(entry),

                                    CreatedAt = now
                                });

                            break;
                        }

                    case EntityState.Modified:
                        {
                            entity.UpdatedAt = now;

                            entity.UpdatedBy =
                                _currentUser.UserId;

                            var isSoftDelete =
                                entry.Properties.Any(
                                    property =>
                                        property.Metadata.Name ==
                                            nameof(
                                                AuditableEntity.IsDeleted)
                                        &&
                                        property.IsModified
                                        &&
                                        property.OriginalValue
                                            is bool oldValue
                                        &&
                                        property.CurrentValue
                                            is bool newValue
                                        &&
                                        !oldValue
                                        &&
                                        newValue);

                            auditLogs.Add(
                                new AuditLog
                                {
                                    TenantId = entity.TenantId,

                                    UserId =
                                        _currentUser.UserId,

                                    EntityName =
                                        entry.Metadata.ClrType.Name,

                                    ActionType =
                                        isSoftDelete
                                            ? "SoftDelete"
                                            : "Update",

                                    RecordId =
                                        entity.Id.ToString(),

                                    OldValues =
                                        AuditValueSerializer
                                            .GetOldValues(entry),

                                    NewValues =
                                        AuditValueSerializer
                                            .GetNewValues(entry),

                                    CreatedAt = now
                                });

                            break;
                        }

                    case EntityState.Deleted:
                        {
                            auditLogs.Add(
                                new AuditLog
                                {
                                    TenantId = entity.TenantId,

                                    UserId =
                                        _currentUser.UserId,

                                    EntityName =
                                        entry.Metadata.ClrType.Name,

                                    ActionType = "Delete",

                                    RecordId =
                                        entity.Id.ToString(),

                                    OldValues =
                                        AuditValueSerializer
                                            .GetCreatedValues(entry),

                                    NewValues = null,

                                    CreatedAt = now
                                });

                            break;
                        }
                }
            }

            foreach (var entry in saleItemEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        {
                            auditLogs.Add(
                                new AuditLog
                                {
                                    TenantId =
                                        entry.Entity.TenantId,

                                    UserId =
                                        _currentUser.UserId,

                                    EntityName =
                                        nameof(SaleItem),

                                    ActionType = "Create",

                                    RecordId =
                                        entry.Entity.Id.ToString(),

                                    OldValues = null,

                                    NewValues =
                                        AuditValueSerializer
                                            .GetCreatedValues(entry),

                                    CreatedAt = now
                                });

                            break;
                        }

                    case EntityState.Deleted:
                        {
                            auditLogs.Add(
                                new AuditLog
                                {
                                    TenantId =
                                        entry.Entity.TenantId,

                                    UserId =
                                        _currentUser.UserId,

                                    EntityName =
                                        nameof(SaleItem),

                                    ActionType = "Delete",

                                    RecordId =
                                        entry.Entity.Id.ToString(),

                                    OldValues =
                                        AuditValueSerializer
                                            .GetCreatedValues(entry),

                                    NewValues = null,

                                    CreatedAt = now
                                });

                            break;
                        }
                }
            }

            foreach (var entry in purchaseItemEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        {
                            auditLogs.Add(
                                new AuditLog
                                {
                                    TenantId =
                                        entry.Entity.TenantId,

                                    UserId =
                                        _currentUser.UserId,

                                    EntityName =
                                        nameof(PurchaseItem),

                                    ActionType = "Create",

                                    RecordId =
                                        entry.Entity.Id.ToString(),

                                    OldValues = null,

                                    NewValues =
                                        AuditValueSerializer
                                            .GetCreatedValues(entry),

                                    CreatedAt = now
                                });

                            break;
                        }

                    case EntityState.Deleted:
                        {
                            auditLogs.Add(
                                new AuditLog
                                {
                                    TenantId =
                                        entry.Entity.TenantId,

                                    UserId =
                                        _currentUser.UserId,

                                    EntityName =
                                        nameof(PurchaseItem),

                                    ActionType = "Delete",

                                    RecordId =
                                        entry.Entity.Id.ToString(),

                                    OldValues =
                                        AuditValueSerializer
                                            .GetCreatedValues(entry),

                                    NewValues = null,

                                    CreatedAt = now
                                });

                            break;
                        }
                }
            }

            if (auditLogs.Count > 0)
            {
                await AuditLogs.AddRangeAsync(
                    auditLogs,
                    cancellationToken);
            }

            var autoDetectChangesEnabled =
                ChangeTracker.AutoDetectChangesEnabled;

            try
            {
                ChangeTracker.AutoDetectChangesEnabled = false;

                return await base.SaveChangesAsync(
                    cancellationToken);
            }
            finally
            {
                ChangeTracker.AutoDetectChangesEnabled =
                    autoDetectChangesEnabled;
            }
        }

        public void ClearChangeTracker()
        {
            ChangeTracker.Clear();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AppDbContext).Assembly);

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

            modelBuilder.Entity<AuditLog>()
                .HasIndex(x => x.TenantId);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            return await Database.BeginTransactionAsync(
                cancellationToken);
        }
    }
}
