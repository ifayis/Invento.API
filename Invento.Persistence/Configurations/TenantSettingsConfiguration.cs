using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invento.Persistence.Configurations;

public class TenantSettingsConfiguration
    : IEntityTypeConfiguration<TenantSettings>
{
    public void Configure(
        EntityTypeBuilder<TenantSettings> builder)
    {
        builder.ToTable("TenantSettings");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.TenantId)
            .IsUnique();

        builder.Property(x => x.MonthlySalesTarget)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.MonthlyProfitTarget)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(x => x.Tenant)
            .WithOne()
            .HasForeignKey<TenantSettings>(
                x => x.TenantId);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}