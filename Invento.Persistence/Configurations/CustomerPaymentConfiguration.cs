using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invento.Persistence.Configurations
{
    public class CustomerPaymentConfiguration
        : IEntityTypeConfiguration<CustomerPayment>
    {
        public void Configure(
            EntityTypeBuilder<CustomerPayment> builder)
        {
            builder.ToTable("CustomerPayments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Remarks)
                .HasMaxLength(1000);

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Sale)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.SaleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x =>
                new
                {
                    x.TenantId,
                    x.CustomerId,
                    x.IsDeleted
                });

            builder.HasIndex(x =>
                new
                {
                    x.TenantId,
                    x.SaleId,
                    x.IsDeleted
                });

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}