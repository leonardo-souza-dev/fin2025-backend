using System.Net;
using System.Net.Http.Json;
using Fin.Api.IntegrationTests.Base;
using Fin.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Fin.Infrastructure.Data;

namespace Fin.Api.IntegrationTests.Base;

/// <summary>
/// Exemplo de teste de integração usando TestContainers
/// Este exemplo demonstra os principais padrões e práticas
/// </summary>
public class ExampleIntegrationTests : IntegrationTestBase
{
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
        
        var newConfig = new Config 
        { 
            Key = "fooKey", 
            Value = "fooValue",
            IsActive = true
        };
        dbContext.Configs.Add(newConfig);
        await dbContext.SaveChangesAsync();

        _ = await LoginAndSetAccessTokenAsync();

        // Act
        var response = await Client.GetAsync("/api/configs");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        // Verify the new bank was created and is returned
        var configs = await response.Content.ReadFromJsonAsync<List<dynamic>>();
        Assert.That(configs, Is.Not.Null);
        Assert.That(configs, Has.Count.EqualTo(2)); // 1 seeded + 1 custom
    }

    [Test]
    public async Task Example_ErrorScenario_ShouldHandleErrorsCorrectly()
    {
        // Arrange
        _ = await LoginAndSetAccessTokenAsync();

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
        var response = await Client.GetAsync("/api/bank-accounts");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}
