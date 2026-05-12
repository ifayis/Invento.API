using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Persistence.Configurations
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SaleNumber)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.SubTotal)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TaxAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TotalAmount)
                .HasColumnType("decimal(18,2)");

            builder.HasIndex(x =>
                new
                {
                    x.TenantId,
                    x.SaleNumber
                })
                .IsUnique();

            builder.HasIndex(x => x.SaleDate);

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Sales)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
