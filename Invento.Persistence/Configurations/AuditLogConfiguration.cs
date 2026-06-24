using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invento.Persistence.Configurations
{
    public class AuditLogConfiguration
        : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(
            EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EntityName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.ActionType)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.RecordId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.OldValues)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.NewValues)
                .HasColumnType("nvarchar(max)");

            builder.HasIndex(x => x.TenantId);

            builder.HasIndex(x => x.CreatedAt);
        }
    }
}