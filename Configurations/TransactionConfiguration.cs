using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BankManagement.Models;

namespace BankManagement.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.TransactionNumber);

        builder.Property(t => t.TransactionDate).IsRequired();
        builder.Property(t => t.Amount).IsRequired().HasPrecision(18, 2);
        builder.Property(t => t.TransactionType).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Note).HasMaxLength(250);
        builder.Property(t => t.AccountNumber).IsRequired().HasMaxLength(20);

        builder.HasOne(t => t.Account)
               .WithMany(a => a.Transactions)
               .HasForeignKey(t => t.AccountNumber)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
