using Microsoft.EntityFrameworkCore;
using BankManagement.Models;

namespace BankManagement.Data;

public class BankContext : DbContext
{
    public DbSet<Branch> Branches { get; set; } = null!;
    public DbSet<Manager> Managers { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<CustomerAccount> CustomerAccounts { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            Environment.GetEnvironmentVariable("BANK_CONNECTION_STRING")
            ?? "Server=.;Database=BankManagementDb;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankContext).Assembly);

        modelBuilder.Seed();
    }
}
