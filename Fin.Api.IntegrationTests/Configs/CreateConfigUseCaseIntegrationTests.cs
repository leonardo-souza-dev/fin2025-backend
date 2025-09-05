using System.Net;
using System.Net.Http.Json;
using Fin.Api.IntegrationTests.Base;
using Fin.Application.UseCases.Configs;

namespace Fin.Api.IntegrationTests.Configs;

public class CreateConfigUseCaseIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task MustCreateConfig()
    {
        // Arrange
        _ = await LoginAndSetAccessTokenAsync();

        // Act
        var postResponse = await Client.PostAsJsonAsync("/api/configs", new CreateConfigRequest
        {
            Key = "foo",
            Value = "baz"
        });
        
        // Assert
        Assert.That(postResponse, Is.Not.Null);
        Assert.That(postResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var response = await postResponse.Content.ReadFromJsonAsync<CreateConfigResponse>();
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response.Key, Is.EqualTo("foo"));
            Assert.That(response.Value, Is.EqualTo("baz"));
            Assert.That(response.Id, Is.GreaterThan(0));
        });
    }
    
}