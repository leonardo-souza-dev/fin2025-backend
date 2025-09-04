using System.Net;
using System.Net.Http.Json;
using Fin.Api.IntegrationTests.Base;
using Fin.Domain.Entities;

namespace Fin.Api.IntegrationTests.Accounts;

public class GetAccountsUseCaseIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task ShouldGetAllAccounts()
    {
        // Arrange
        _ = await LoginAndSetAccessTokenAsync();

        // Act
        var response = await Client.GetAsync("/api/bank-accounts/accounts");
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var entities = await response.Content.ReadFromJsonAsync<List<dynamic>>();
        Assert.That(entities, Is.Not.Null);
        Assert.That(entities, Is.Not.Empty);
    }
}