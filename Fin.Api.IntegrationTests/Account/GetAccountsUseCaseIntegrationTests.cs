using System.Net;
using System.Net.Http.Json;
using Fin.Api.IntegrationTests.Base;
using Fin.Domain.Entities;

namespace Fin.Api.IntegrationTests.Account;

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
        var getAccountsResponse = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<Domain.Entities.Account>>();
        Assert.That(getAccountsResponse, Is.Not.Null);
        Assert.That(getAccountsResponse.Count, Is.GreaterThan(0));
    }
}