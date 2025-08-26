using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Fin.Infrastructure.Data;

namespace Fin.Api.IntegrationTests;

/// <summary>
/// Exemplo de teste de integração usando TestContainers
/// Este exemplo demonstra os principais padrões e práticas
/// </summary>
public class ExampleIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task Example_GetAccounts_ShouldReturnSeedAccounts()
    {
        // Arrange
        await SetAccessTokenAsync();

        // Act
        var response = await Client.GetAsync("/api/accounts");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var accounts = await response.Content.ReadFromJsonAsync<List<dynamic>>();
        Assert.That(accounts, Is.Not.Null);
        Assert.That(accounts.Count, Is.EqualTo(2)); // We seeded 2 accounts
    }

    [Test]
    public void Example_AccessDatabase_ShouldAllowDirectDatabaseAccess()
    {
        // Arrange & Act - Access database directly if needed
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FinDbContext>();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(dbContext.Users.ToList(), Has.Count.EqualTo(1));
            Assert.That(dbContext.Banks.ToList(), Has.Count.EqualTo(2));
            Assert.That(dbContext.Accounts.ToList(), Has.Count.EqualTo(2));
        });
    }

    [Test]
    public async Task Example_AddCustomTestData_ShouldAllowDataManipulation()
    {
        // Arrange - Add additional test data for this specific test
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FinDbContext>();
        
        var newBank = new Fin.Domain.Entities.Bank 
        { 
            Name = "Custom Test Bank", 
            IsActive = true 
        };
        dbContext.Banks.Add(newBank);
        await dbContext.SaveChangesAsync();

        await SetAccessTokenAsync();

        // Act
        var response = await Client.GetAsync("/api/banks");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        // Verify the new bank was created and is returned
        var banks = await response.Content.ReadFromJsonAsync<List<dynamic>>();
        Assert.That(banks, Is.Not.Null);
        Assert.That(banks.Count, Is.EqualTo(3)); // 2 seeded + 1 custom
    }

    [Test]
    public async Task Example_ErrorScenario_ShouldHandleErrorsCorrectly()
    {
        // Arrange
        await SetAccessTokenAsync();

        // Act - Try to access a non-existent resource
        var response = await Client.GetAsync("/api/accounts/99999");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Example_UnauthorizedAccess_ShouldRequireAuthentication()
    {
        // Arrange - Don't set access token

        // Act
        var response = await Client.GetAsync("/api/accounts");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}
