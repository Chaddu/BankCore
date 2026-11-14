using BankCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankCore.Infrastructure.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.EventId)
            .IsRequired();

        builder.HasIndex(o => o.EventId)
            .IsUnique();

        builder.Property(o => o.Type)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Content)
            .IsRequired();

        builder.Property(o => o.OccurredAt)
            .IsRequired();

        builder.Property(o => o.ProcessedAt);

        builder.HasIndex(o => new { o.ProcessedAt, o.OccurredAt });

        builder.Property(o => o.RetryCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(o => o.LastError)
            .HasMaxLength(2000);

        builder.Property(o => o.ClaimId);

        builder.Property(o => o.ClaimedUntil);

        builder.HasIndex(o => new { o.ClaimedUntil, o.ProcessedAt });

        builder.Property(o => o.FailedAt);

        builder.HasIndex(o => o.FailedAt);

    }
}
