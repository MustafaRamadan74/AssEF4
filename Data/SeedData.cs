using Microsoft.EntityFrameworkCore;
using BankManagement.Models;

namespace BankManagement.Data;

public static class SeedData
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Branch>().HasData(
            new Branch { Code = "CAI-01", Name = "Cairo Main Branch", Address = "12 Tahrir Square, Cairo", PhoneNumber = "0225550101" },
            new Branch { Code = "ALX-01", Name = "Alexandria Branch", Address = "5 Corniche Road, Alexandria", PhoneNumber = "0335550202" }
        );

        modelBuilder.Entity<Manager>().HasData(
            new Manager { Id = 1, FullName = "Hossam Fathy", Email = "hossam.fathy@nationalbank.example", PhoneNumber = "01012345678", HireDate = new DateTime(2018, 3, 1), BranchCode = "CAI-01" },
            new Manager { Id = 2, FullName = "Rania Gamal", Email = "rania.gamal@nationalbank.example", PhoneNumber = "01098765432", HireDate = new DateTime(2020, 9, 15), BranchCode = "ALX-01" }
        );

        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, FullName = "Ahmed Ali", Email = "ahmed.ali@example.com", PhoneNumber = "01111111111", DateOfBirth = new DateTime(1990, 5, 10), Address = "10 Nile Street, Cairo", NationalId = "29005101234567", CustomerType = CustomerType.Individual },
            new Customer { Id = 2, FullName = "Nile Trading LLC", Email = "info@niletrading.example", PhoneNumber = "0222223333", DateOfBirth = new DateTime(2010, 1, 1), Address = "Free Zone, Alexandria", NationalId = "30001015550001", CustomerType = CustomerType.Business }
        );

        modelBuilder.Entity<Account>().HasData(
            new Account { AccountNumber = "2001-CUR", CurrentBalance = 15400.00m, AccountType = AccountType.Current, OpeningDate = new DateTime(2023, 5, 1), BranchCode = "CAI-01" },
            new Account { AccountNumber = "3000-BUS", CurrentBalance = 120000.00m, AccountType = AccountType.Business, OpeningDate = new DateTime(2022, 11, 20), BranchCode = "ALX-01" }
        );

        modelBuilder.Entity<CustomerAccount>().HasData(
            new CustomerAccount { CustomerId = 1, AccountNumber = "2001-CUR", OwnershipType = OwnershipType.Primary, OwnershipStartDate = new DateTime(2023, 5, 1), AccountStatus = AccountStatus.Active },
            new CustomerAccount { CustomerId = 2, AccountNumber = "3000-BUS", OwnershipType = OwnershipType.Primary, OwnershipStartDate = new DateTime(2022, 11, 20), AccountStatus = AccountStatus.Active }
        );
    }
}
