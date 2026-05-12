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
    public class StockMovementConfiguration
        : IEntityTypeConfiguration<StockMovement>
    {
        public void Configure(EntityTypeBuilder<StockMovement> builder)
        {
            builder.ToTable("StockMovements");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReferenceType)
                .HasMaxLength(100);

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            builder.HasIndex(x =>
                new
                {
                    x.TenantId,
                    x.ProductId
                });

            builder.HasIndex(x => x.CreatedAt);

            builder.HasOne(x => x.Product)
                .WithMany(x => x.StockMovements)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
