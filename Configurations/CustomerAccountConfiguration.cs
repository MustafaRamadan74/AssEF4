using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BankManagement.Models;

namespace BankManagement.Configurations;

public class CustomerAccountConfiguration : IEntityTypeConfiguration<CustomerAccount>
{
    public void Configure(EntityTypeBuilder<CustomerAccount> builder)
    {
        builder.ToTable("CustomerAccounts");

        builder.HasKey(ca => new { ca.CustomerId, ca.AccountNumber });

        builder.Property(ca => ca.AccountNumber).IsRequired().HasMaxLength(20);
        builder.Property(ca => ca.OwnershipType).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(ca => ca.OwnershipStartDate).IsRequired().HasColumnType("date");
        builder.Property(ca => ca.AccountStatus).IsRequired().HasConversion<string>().HasMaxLength(20);

        builder.HasOne(ca => ca.Customer)
               .WithMany(c => c.CustomerAccounts)
               .HasForeignKey(ca => ca.CustomerId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ca => ca.Account)
               .WithMany(a => a.CustomerAccounts)
               .HasForeignKey(ca => ca.AccountNumber)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
