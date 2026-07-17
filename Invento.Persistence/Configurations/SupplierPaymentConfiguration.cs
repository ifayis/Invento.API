using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invento.Persistence.Configurations
{
    public class SupplierPaymentConfiguration
        : IEntityTypeConfiguration<SupplierPayment>
    {
        public void Configure(
            EntityTypeBuilder<SupplierPayment> builder)
        {
            builder.ToTable("SupplierPayments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Remarks)
                .HasMaxLength(1000);

            builder.HasOne(x => x.Supplier)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Purchase)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.PurchaseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x =>
                new
                {
                    x.TenantId,
                    x.SupplierId,
                    x.IsDeleted
                });

            builder.HasIndex(x =>
                new
                {
                    x.TenantId,
                    x.PurchaseId,
                    x.IsDeleted
                });

            builder.HasQueryFilter(
                x => !x.IsDeleted);
        }
    }
}