using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invento.Persistence.Configurations
{
    public class SalesConfiguration
        : IEntityTypeConfiguration<Sale>
    {
        public void Configure(
            EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.SubTotal)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TaxAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TotalAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.ProfitAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.PaidAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DueAmount)
                .HasColumnType("decimal(18,2)");

            builder.HasIndex(x =>
                new
                {
                    x.TenantId,
                    x.InvoiceNumber
                })
                .IsUnique();

            builder.HasIndex(x =>
                new
                {
                    x.TenantId,
                    x.IsDeleted,
                    x.SaleDate
                });

            builder.HasIndex(x =>
                new
                {
                    x.TenantId,
                    x.CustomerId,
                    x.IsDeleted,
                    x.SaleDate
                });

            builder.HasMany(x => x.SaleItems)
                .WithOne(x => x.Sale)
                .HasForeignKey(x => x.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Payments)
                .WithOne(x => x.Sale)
                .HasForeignKey(x => x.SaleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(
                x => !x.IsDeleted);
        }
    }
}