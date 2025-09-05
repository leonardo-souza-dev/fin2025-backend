using System.Net;
using System.Net.Http.Json;
using Fin.Api.IntegrationTests.Base;
using Fin.Domain.Entities;

namespace Fin.Api.IntegrationTests.Configs;

public class GetConfigUseCaseIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task ShouldGetAllActiveConfigs()
    {
        // Arrange
        _ = await LoginAndSetAccessTokenAsync();

        // Act
        var httpResponseMessage = await Client.GetAsync("/api/configs");
        
        // Assert
        Assert.That(httpResponseMessage, Is.Not.Null);
        Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<Config>>();
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Count, Is.GreaterThan(0));
    }
}