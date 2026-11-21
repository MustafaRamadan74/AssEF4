using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BankManagement.Data;
using BankManagement.Helpers;
using BankManagement.Models;

namespace BankManagement;

public class Program
{
    public static void Main()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        Console.OutputEncoding = Encoding.UTF8;

        using var context = new BankContext();
        context.Database.Migrate();

        while (true)
        {
            ShowMenu();
            var choice = Console.ReadLine()?.Trim();

            if (choice == "0")
            {
                break;
            }

            try
            {
                switch (choice)
                {
                    case "1": AddCustomer(context); break;
                    case "2": OpenAccount(context); break;
                    case "3": UpdateAccountStatus(context); break;
                    case "4": RemoveAccountFromCustomer(context); break;
                    case "5": ListCustomers(context); break;
                    default: ConsoleUi.Error("  Invalid choice. Please enter a number from 0 to 5."); break;
                }
            }
            catch (DbUpdateException ex)
            {
                context.ChangeTracker.Clear();
                ConsoleUi.Error($"  Database error: {ex.InnerException?.Message ?? ex.Message}");
            }

            ConsoleUi.Pause();
        }

        Console.WriteLine("Goodbye.");
    }

    private static void ShowMenu()
    {
        ConsoleUi.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("    National Bank — Management");
        Console.WriteLine("========================================");
        Console.WriteLine("  1) Add a new Customer");
        Console.WriteLine("  2) Open a new Account for a Customer");
        Console.WriteLine("  3) Update Account Status (Active / Closed)");
        Console.WriteLine("  4) Remove an Account from a Customer");
        Console.WriteLine("  5) List all Customers (with accounts)");
        Console.WriteLine("  0) Exit");
        Console.WriteLine("----------------------------------------");
        Console.Write("  Enter choice: ");
    }

    private static void AddCustomer(BankContext context)
    {
        Console.WriteLine();
        Console.WriteLine("--- Add New Customer ---");

        var fullName = ConsoleUi.ReadText("Full Name", 100);
        var nationalId = ConsoleUi.ReadDigits("National ID", 10, 14);
        var dateOfBirth = ConsoleUi.ReadPastDate("Date of Birth");
        var email = ConsoleUi.ReadEmail("Email", 150);
        var phone = ConsoleUi.ReadPhone("Phone");
        var address = ConsoleUi.ReadText("Address", 200);
        var customerType = ConsoleUi.ReadEnum<CustomerType>("Customer Type");

        if (context.Customers.Any(c => c.NationalId == nationalId))
        {
            Console.WriteLine();
            ConsoleUi.Error($"  A customer with National ID {nationalId} already exists.");
            return;
        }

        var customer = new Customer
        {
            FullName = fullName,
            NationalId = nationalId,
            DateOfBirth = dateOfBirth,
            Email = email,
            PhoneNumber = phone,
            Address = address,
            CustomerType = customerType
        };

        context.Customers.Add(customer);
        context.SaveChanges();

        Console.WriteLine();
        ConsoleUi.Success($"Customer created successfully. CustomerId = {customer.Id}");
    }

    private static void OpenAccount(BankContext context)
    {
        Console.WriteLine();
        Console.WriteLine("--- Open New Account ---");

        var accountNumber = ConsoleUi.ReadCode("Account Number", 20);
        var accountType = ConsoleUi.ReadEnum<AccountType>("Account Type");
        var branchCode = ConsoleUi.ReadCode("Branch Code", 10);
        var customerId = ConsoleUi.ReadPositiveInt("Customer Id");
        var role = ConsoleUi.ReadEnum<OwnershipType>("Ownership Role");

        ConsoleUi.Info($"Validating branch '{branchCode}' and customer #{customerId}...");

        if (!context.Branches.Any(b => b.Code == branchCode))
        {
            ConsoleUi.Error($"  Branch '{branchCode}' does not exist.");
            return;
        }

        if (!context.Customers.Any(c => c.Id == customerId))
        {
            ConsoleUi.Error($"  Customer #{customerId} does not exist.");
            return;
        }

        var account = context.Accounts.Find(accountNumber);
        var isNewAccount = account is null;

        if (account is null)
        {
            if (role != OwnershipType.Primary)
            {
                ConsoleUi.Error("  A new account must start with a Primary owner.");
                return;
            }

            context.Accounts.Add(new Account
            {
                AccountNumber = accountNumber,
                AccountType = accountType,
                BranchCode = branchCode,
                CurrentBalance = 0m,
                OpeningDate = DateTime.Today
            });
        }
        else
        {
            if (account.BranchCode != branchCode)
            {
                ConsoleUi.Error($"  Account '{accountNumber}' belongs to branch '{account.BranchCode}', not '{branchCode}'.");
                return;
            }

            if (context.CustomerAccounts.Any(ca => ca.AccountNumber == accountNumber && ca.CustomerId == customerId))
            {
                ConsoleUi.Error($"  Customer #{customerId} is already linked to account '{accountNumber}'.");
                return;
            }

            if (role == OwnershipType.Primary
                && context.CustomerAccounts.Any(ca => ca.AccountNumber == accountNumber && ca.OwnershipType == OwnershipType.Primary))
            {
                ConsoleUi.Error($"  Account '{accountNumber}' already has a Primary owner.");
                return;
            }
        }

        context.CustomerAccounts.Add(new CustomerAccount
        {
            CustomerId = customerId,
            AccountNumber = accountNumber,
            OwnershipType = role,
            OwnershipStartDate = DateTime.Today,
            AccountStatus = AccountStatus.Active
        });
        context.SaveChanges();

        if (isNewAccount)
        {
            ConsoleUi.Success($"Account '{accountNumber}' created and linked to customer {customerId} as {role} owner.");
        }
        else
        {
            ConsoleUi.Success($"Existing account '{accountNumber}' linked to customer {customerId} as {role} owner.");
        }
    }

    private static void UpdateAccountStatus(BankContext context)
    {
        Console.WriteLine();
        Console.WriteLine("--- Update Account Status ---");

        var accountNumber = ConsoleUi.ReadCode("Account Number", 20);
        var customerId = ConsoleUi.ReadPositiveInt("Customer Id");
        var newStatus = ConsoleUi.ReadEnum<AccountStatus>("New Status");

        var link = context.CustomerAccounts.Find(customerId, accountNumber);
        if (link is null)
        {
            ConsoleUi.Error($"  Account '{accountNumber}' is not linked to customer #{customerId}.");
            return;
        }

        if (link.AccountStatus == newStatus)
        {
            ConsoleUi.Info($"  The status is already {newStatus}. Nothing to update.");
            return;
        }

        link.AccountStatus = newStatus;
        context.SaveChanges();

        ConsoleUi.Success($"Status updated to {newStatus}.");
    }

    private static void RemoveAccountFromCustomer(BankContext context)
    {
        Console.WriteLine();
        Console.WriteLine("--- Remove Account From Customer ---");

        var accountNumber = ConsoleUi.ReadCode("Account Number", 20);
        var customerId = ConsoleUi.ReadPositiveInt("Customer Id");

        var link = context.CustomerAccounts.Find(customerId, accountNumber);
        if (link is null)
        {
            Console.WriteLine();
            ConsoleUi.Error($"  Account '{accountNumber}' is not linked to customer #{customerId}.");
            return;
        }

        context.CustomerAccounts.Remove(link);
        context.SaveChanges();

        Console.WriteLine();
        ConsoleUi.Success("  Ownership link deleted.");

        if (context.CustomerAccounts.Any(ca => ca.AccountNumber == accountNumber))
        {
            return;
        }

        var transactionCount = context.Transactions.Count(t => t.AccountNumber == accountNumber);
        if (transactionCount > 0)
        {
            ConsoleUi.Info($"      That was the last owner, but account '{accountNumber}' was kept because it has {transactionCount} transaction(s).");
            return;
        }

        var account = context.Accounts.Find(accountNumber);
        if (account is not null)
        {
            context.Accounts.Remove(account);
            context.SaveChanges();
            ConsoleUi.Info($"      That was the last owner — account '{accountNumber}' was also removed.");
        }
    }

    private static void ListCustomers(BankContext context)
    {
        Console.WriteLine();
        Console.WriteLine("--- All Customers ---");

        var customers = context.Customers
            .AsNoTracking()
            .Include(c => c.CustomerAccounts)
                .ThenInclude(ca => ca.Account)
                    .ThenInclude(a => a.Branch)
            .OrderBy(c => c.Id)
            .ToList();

        if (customers.Count == 0)
        {
            ConsoleUi.Info("  (no customers yet)");
            return;
        }

        Console.WriteLine();
        foreach (var customer in customers)
        {
            Console.WriteLine($"  #{customer.Id} {customer.FullName} ({customer.CustomerType})");

            if (customer.CustomerAccounts.Count == 0)
            {
                Console.WriteLine("      (no accounts)");
                continue;
            }

            foreach (var link in customer.CustomerAccounts.OrderBy(ca => ca.AccountNumber))
            {
                var account = link.Account;
                Console.WriteLine($"      {account.AccountNumber,-10}{account.AccountType,-10}Balance: {account.CurrentBalance,12:N2}  {link.OwnershipType,-9}{link.AccountStatus,-8}@ {account.Branch.Name}");
            }
        }
    }
}
