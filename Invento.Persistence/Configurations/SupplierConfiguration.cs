using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invento.Persistence.Configurations
{
    public class SupplierConfiguration
        : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(
            EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.ContactPerson)
                .HasMaxLength(200);

            builder.Property(x => x.Email)
                .HasMaxLength(200);

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(50);

            builder.Property(x => x.Address)
                .HasMaxLength(1000);

            builder.Property(x => x.TaxRegistrationNumber)
                .HasMaxLength(100);

            builder.HasIndex(x =>
                new
                {
                    x.TenantId,
                    x.IsDeleted,
                    x.CreatedAt,
                    x.Id
                });

            builder.HasQueryFilter(
                x => !x.IsDeleted);
        }
    }
}