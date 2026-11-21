# National Bank Management System

EF Core Assignment 04. A .NET 8 console app for customers, accounts and shared ownership, using SQL Server and EF Core Code First.

## Run

Install the .NET 8 SDK and start a SQL Server instance. From this folder run:

```sh
dotnet run
```

On Windows the default connection uses the local default SQL Server instance, Windows authentication and database BankManagementDb. Use an account that can create the database and tables. To use SQL Express and a separate assignment database, set this in PowerShell before running:

```powershell
$env:BANK_CONNECTION_STRING = 'Server=.\SQLEXPRESS;Database=BankAssignment04;Trusted_Connection=True;TrustServerCertificate=True;'
```

The first launch applies the included migration and seeds branches CAI-01 and ALX-01, their managers, and two sample customers with accounts. Choose 5 to view them. Choose 1 to add a customer and 2 to open an account. To share an existing account, use its number and branch with a different customer and select CoHolder. Option 3 updates the selected customer's ownership status. Option 4 removes the ownership link and also removes an ownerless account if it has no transactions.
