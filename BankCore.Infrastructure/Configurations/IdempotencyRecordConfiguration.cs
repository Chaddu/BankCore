using BankCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankCore.Infrastructure.Configurations;

public class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Key)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasIndex(i => i.Key)
            .IsUnique();

        builder.Property(i => i.RequestHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(i => i.TransactionId)
            .IsRequired();

        builder.Property(i => i.ExpiresAt)
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .IsRequired();
    }
}
