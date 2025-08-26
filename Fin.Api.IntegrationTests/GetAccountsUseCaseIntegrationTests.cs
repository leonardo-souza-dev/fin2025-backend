using System.Net;
using System.Net.Http.Json;
using Fin.Domain.Entities;

namespace Fin.Api.IntegrationTests;

public class GetAccountsUseCaseIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task ShouldGetAllActiveAccounts()
    {
        // Arrange
        _ = await LoginAndSetAccessTokenAsync();

        // Act
        var httpResponseMessage = await Client.GetAsync("/api/accounts");
        
        // Assert
        Assert.That(httpResponseMessage, Is.Not.Null);
        Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var getAccountsResponse = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<Account>>();
        Assert.That(getAccountsResponse, Is.Not.Null);
        Assert.That(getAccountsResponse.Count, Is.GreaterThan(0));
    }
}