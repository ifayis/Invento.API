using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invento.Persistence.Configurations
{
    public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
    {
        public void Configure(EntityTypeBuilder<SaleItem> builder)
        {
            builder.ToTable("SaleItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UnitPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.CostPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TaxRate)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TaxAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TotalPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.ProfitAmount)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.Product)
                .WithMany(x => x.SaleItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Sale)
                .WithMany(x => x.SaleItems)
                .HasForeignKey(x => x.SaleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}