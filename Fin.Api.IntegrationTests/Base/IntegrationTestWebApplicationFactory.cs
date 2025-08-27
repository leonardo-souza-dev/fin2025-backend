using Fin.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Fin.Infrastructure.Data;

namespace Fin.Api.IntegrationTests.Base;

public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .WithDatabase("fintest")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithCleanUp(true)
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            services.RemoveAll(typeof(DbContextOptions<FinDbContext>));
            services.RemoveAll(typeof(FinDbContext));

            // Add DbContext with TestContainer connection string
            services.AddDbContext<FinDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));

            // Ensure the database is created and seeded
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinDbContext>();
            
            dbContext.Database.EnsureCreated();
            
            // Seed test data
            SeedTestData(dbContext);
        });

        // Set environment variables that the application expects
        builder.UseSetting("JwtSettings:Issuer", "FinApi");
        builder.UseSetting("JwtSettings:Audience", "FinClients");
        builder.UseSetting("JwtSettings:AccessTokenExpiryInMinutes", "60");
        builder.UseSetting("JwtSettings:RefreshTokenExpiryInDays", "7");
        
        builder.UseEnvironment("Testing");
    }

    public async Task InitializeAsync()
    {
        // Set environment variables before starting the container
        Environment.SetEnvironmentVariable("FIN2025_JWT_SECRET_KEY", "Ck4OWSmg6Bto3zr3hEky/PWSSOxjk3cB9LjWQC/aloE=");
        Environment.SetEnvironmentVariable("FIN2025_DATABASE_CONNECTION", "temp-will-be-replaced");
        
        await _dbContainer.StartAsync();
        
        // Update the connection string environment variable with the actual container connection string
        Environment.SetEnvironmentVariable("FIN2025_DATABASE_CONNECTION", _dbContainer.GetConnectionString());
    }

    public async Task StopAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _dbContainer.DisposeAsync().AsTask().Wait();
        }
        base.Dispose(disposing);
    }

    private static void SeedTestData(FinDbContext dbContext)
    {
        // Check if data already exists
        if (dbContext.Users.Any())
            return;

        // Add test user
        var user = new Fin.Domain.Entities.User
        {
            Email = "user@email.com",
            Password = "$2a$13$PsfhXFM6RKlHoNl6Il3MJuTOoaMj.yBEQOyDpjNIy0ZjKgQYp/L/e", // 12345678 hashed
            IsActive = true
        };
        dbContext.Users.Add(user);

        // Add test banks
        var bank1 = new Bank { Name = "Test Bank 1", IsActive = true };
        var bank2 = new Bank { Name = "Test Bank 2", IsActive = true };
        dbContext.Banks.Add(bank1);
        dbContext.Banks.Add(bank2);
        dbContext.SaveChanges(); // Save to get IDs

        // Add test accounts
        var account16 = new Fin.Domain.Entities.Account { BankId = bank1.Id, Name = "Test Account 16", IsActive = true };
        var account62 = new Fin.Domain.Entities.Account { BankId = bank2.Id, Name = "Test Account 62", IsActive = true };
        dbContext.Accounts.Add(account16);
        dbContext.Accounts.Add(account62);
        dbContext.SaveChanges(); // Save to get IDs

        // Ensure specific IDs for testing
        // We need account with ID 16 and 62 for the tests to work
        var existingAccount16 = dbContext.Accounts.FirstOrDefault();
        var existingAccount62 = dbContext.Accounts.Skip(1).FirstOrDefault();
        
        if (existingAccount16 != null && existingAccount62 != null)
        {
            // Add test payments that represent a transfer
            var paymentFrom = new Fin.Domain.Entities.Payment
            {
                Date = new DateOnly(2025, 8, 23),
                Description = "transfer87",
                FromAccountId = existingAccount16.Id!.Value,
                Amount = -10, // Negative because it's going out
                ToAccountId = existingAccount62.Id!.Value,
                IsActive = true
            };
            dbContext.Payments.Add(paymentFrom);

            var paymentTo = new Fin.Domain.Entities.Payment
            {
                Date = new DateOnly(2025, 8, 23),
                Description = "transfer87",
                FromAccountId = existingAccount62.Id!.Value,
                Amount = 10, // Positive because it's coming in
                ToAccountId = existingAccount16.Id!.Value,
                IsActive = true
            };
            dbContext.Payments.Add(paymentTo);
            dbContext.SaveChanges(); // Save to get payment IDs

            // Add test transfer linking the payments
            var transfer = new Fin.Domain.Entities.Transfer
            {
                PaymentFromId = paymentFrom.Id,
                PaymentToId = paymentTo.Id,
                IsActive = true
            };
            dbContext.Transfers.Add(transfer);
            dbContext.SaveChanges(); // Save to get transfer ID

            // Update payments with transfer ID
            paymentFrom.TransferId = transfer.Id;
            paymentTo.TransferId = transfer.Id;
            dbContext.SaveChanges();
        }
    }
}
