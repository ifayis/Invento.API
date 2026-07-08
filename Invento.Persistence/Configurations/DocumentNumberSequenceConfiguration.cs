using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invento.Persistence.Configurations
{
    public class DocumentNumberSequenceConfiguration
        : IEntityTypeConfiguration<DocumentNumberSequence>
    {
        public void Configure(
            EntityTypeBuilder<DocumentNumberSequence> builder)
        {
            builder.ToTable(
                "DocumentNumberSequences");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DocumentType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.PeriodKey)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.NextNumber)
                .IsRequired();

            builder.HasIndex(
                x => new
                {
                    x.TenantId,
                    x.DocumentType,
                    x.PeriodKey
                })
                .IsUnique();
        }
    }
}