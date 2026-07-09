using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invento.Persistence.Configurations
{
    public class CashTransactionConfiguration
        : IEntityTypeConfiguration<CashTransaction>
    {
        public void Configure(
            EntityTypeBuilder<CashTransaction> builder)
        {
            builder.ToTable("CashTransactions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.HasIndex(x =>
                new
                {
                    x.TenantId,
                    x.IsDeleted,
                    x.TransactionDate,
                    x.Id
                });

            builder.HasQueryFilter(
                x => !x.IsDeleted);
        }
    }
}