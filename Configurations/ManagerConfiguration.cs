using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BankManagement.Models;

namespace BankManagement.Configurations;

public class ManagerConfiguration : IEntityTypeConfiguration<Manager>
{
    public void Configure(EntityTypeBuilder<Manager> builder)
    {
        builder.ToTable("Managers");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.FullName).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Email).IsRequired().HasMaxLength(150);
        builder.Property(m => m.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(m => m.HireDate).IsRequired().HasColumnType("date");
        builder.Property(m => m.BranchCode).IsRequired().HasMaxLength(10);

        builder.HasOne(m => m.Branch)
               .WithOne(b => b.Manager)
               .HasForeignKey<Manager>(m => m.BranchCode)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
