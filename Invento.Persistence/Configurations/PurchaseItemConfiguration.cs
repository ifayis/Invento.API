using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invento.Persistence.Configurations
{
    public class PurchaseItemConfiguration
        : IEntityTypeConfiguration<PurchaseItem>
    {
        public void Configure(
            EntityTypeBuilder<PurchaseItem> builder)
        {
            builder.ToTable("PurchaseItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UnitCost)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TaxRate)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TaxAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TotalPrice)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.Product)
                .WithMany(x => x.PurchaseItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Purchase)
                .WithMany(x => x.PurchaseItems)
                .HasForeignKey(x => x.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(
                x => !x.Product.IsDeleted);
        }
    }
}