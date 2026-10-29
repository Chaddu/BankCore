using BankCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankCore.Infrastructure.Configurations;

public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.AccountNumber)
           .IsRequired()
           .HasMaxLength(34);

        builder.HasIndex(a => a.AccountNumber)
            .IsUnique();

        builder.Property(a => a.OwnerId)
            .IsRequired();

        builder.Property(a => a.Balance)
            .HasPrecision(19, 4)
            .IsRequired();

        builder.Property(a => a.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .IsFixedLength();

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .IsRequired(false);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(a => a.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

