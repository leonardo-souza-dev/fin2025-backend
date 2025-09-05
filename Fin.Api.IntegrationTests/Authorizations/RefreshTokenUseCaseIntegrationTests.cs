using System.Net;
using System.Text;
using Fin.Api.IntegrationTests.Base;

namespace Fin.Api.IntegrationTests.Authorizations;

public class RefreshTokenUseCaseIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task MustRefreshToken()
    {
        // Arrange
        var accessToken = await LoginAndSetAccessTokenAsync();

        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Method = HttpMethod.Post;
        httpRequestMessage.Headers.Add("Cookie", $"refreshToken={accessToken}");
        httpRequestMessage.RequestUri = new Uri(Client.BaseAddress!, "/api/auth/refresh");
        httpRequestMessage.Content = new StringContent("{\"AccessToken\":\"" + accessToken + "\"}", Encoding.UTF8, "application/json");
        
        // Act
        var response = await Client.SendAsync(httpRequestMessage);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.Not.Null);
        });

    }
}