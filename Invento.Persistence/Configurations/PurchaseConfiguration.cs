using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invento.Persistence.Configurations
{
    public class PurchaseConfiguration
        : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(
            EntityTypeBuilder<Purchase> builder)
        {
            builder.ToTable("Purchases");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PurchaseNumber)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.SubTotal)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TaxAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TotalAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.PaidAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DueAmount)
                .HasColumnType("decimal(18,2)");

            builder.HasIndex(x =>
                new
                {
                    x.TenantId,
                    x.PurchaseNumber
                })
                .IsUnique();

            builder.HasIndex(x =>
                new
                {
                    x.TenantId,
                    x.IsDeleted,
                    x.PurchaseDate
                });

            builder.HasIndex(x =>
                new
                {
                    x.TenantId,
                    x.SupplierId,
                    x.IsDeleted,
                    x.PurchaseDate
                });

            builder.HasOne(x => x.Supplier)
                .WithMany(x => x.Purchases)
                .HasForeignKey(x => x.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.PurchaseItems)
                .WithOne(x => x.Purchase)
                .HasForeignKey(x => x.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Payments)
                .WithOne(x => x.Purchase)
                .HasForeignKey(x => x.PurchaseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(
                x => !x.IsDeleted);
        }
    }
}