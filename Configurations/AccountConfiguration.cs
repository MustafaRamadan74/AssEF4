using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BankManagement.Models;

namespace BankManagement.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(a => a.AccountNumber);

        builder.Property(a => a.AccountNumber).HasMaxLength(20);
        builder.Property(a => a.CurrentBalance).IsRequired().HasPrecision(18, 2);
        builder.Property(a => a.AccountType).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.OpeningDate).IsRequired().HasColumnType("date");
        builder.Property(a => a.BranchCode).IsRequired().HasMaxLength(10);

        builder.HasOne(a => a.Branch)
               .WithMany(b => b.Accounts)
               .HasForeignKey(a => a.BranchCode)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
