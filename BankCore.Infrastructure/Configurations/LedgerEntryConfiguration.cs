using BankCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankCore.Infrastructure.Configurations;

public class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
{
    public void Configure(EntityTypeBuilder<LedgerEntry> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.BankAccountId)
            .IsRequired();

        builder.Property(l => l.Amount)
            .HasPrecision(19, 4)
            .IsRequired();

        builder.Property(l => l.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(l => l.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(l => l.TransactionId)
            .IsRequired();

        builder.HasIndex(l => l.TransactionId);

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        builder.HasOne<BankAccount>()
            .WithMany()
            .HasForeignKey(l => l.BankAccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

