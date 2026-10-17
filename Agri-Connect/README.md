# Agri-Connect

ASP.NET Core 8 MVC app (EF Core + SQL Server + ASP.NET Core Identity) where **farmers** manage their own
products and **employees** manage the farmer directory.

## Roles

| Role | How you get it | What you can do |
|---|---|---|
| Farmer | Self-register on the Register page, **or** an Employee adds you | Add / edit / delete / search / filter **your own** products |
| Employee | Created only from configuration (see below); never self-service | Add / edit / delete / search farmers; view and filter any farmer's products |

* Registering always creates a Farmer (role + farmer profile). The Employee role cannot be chosen on the site.
* When an Employee adds a farmer, the app creates the farmer's login too and shows a one-time password **once**.
  Hand it over securely.
* Deleting a farmer also deletes their products and their login.

## First run

1. Requirements: .NET 8 SDK and SQL Server (LocalDB ships with Visual Studio).
2. The default connection string uses LocalDB. To use another server, don't edit `appsettings.json`; set it privately:
   ```
   cd Agri-Connect
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER;Database=Agri-Connect;Trusted_Connection=True;TrustServerCertificate=True"
   ```
3. Create the first Employee account (nothing is hard-coded; pick your own credentials):
   ```
   dotnet user-secrets set "SeedEmployee:Email" "you@example.com"
   dotnet user-secrets set "SeedEmployee:Password" "a-strong-password"
   ```
   In production use environment variables instead: `SeedEmployee__Email` and `SeedEmployee__Password`.
4. `dotnet run`. In Development the database is created/migrated automatically.
   Elsewhere run `dotnet ef database update` before starting.

Passwords must be at least 8 characters.

## Upgrading an existing database

The `AddFarmerOwnership` migration links products to farmers. Products created before it have no owner, so the
migration **deletes existing products**. Farmers keep their rows but have no login until you re-add them or they register.

The old version seeded `employee@agri.com` / `Employee123!` and `farmer@agri.com` / `Farmer123!`. Those accounts
stay in an existing database, so remove them:

```sql
DELETE FROM AspNetUsers WHERE Email IN ('employee@agri.com', 'farmer@agri.com');
```
