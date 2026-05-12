using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Invento.Domain.Entities;

namespace Invento.Persistence.Configurations
{
    public class ProductConfiguration
    : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.SKU)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            builder.Property(x => x.CostPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.SellingPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TaxRate)
                .HasColumnType("decimal(5,2)");

            builder.HasIndex(x =>
                new
                {
                    x.TenantId,
                    x.SKU
                })
                .IsUnique();

            builder.HasIndex(x =>
                new
                {
                    x.TenantId,
                    x.Name
                });

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}