using System.Net;
using System.Net.Http.Json;
using Fin.Api.IntegrationTests.Base;
using Fin.Application.UseCases.Auth;

namespace Fin.Api.IntegrationTests.Auth;

public class LogoutUseCaseIntegrationTest : IntegrationTestBase
{
    [Test]
    public async Task MustLogout()
    {
        // Arrange
        _ = await LoginAndSetAccessTokenAsync();
        
        // Act
        var response = await Client.DeleteAsync("/api/auth/logout");
        
        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var logoutResponse = await response.Content.ReadFromJsonAsync<LogoutResponse>();
        Assert.That(logoutResponse, Is.Not.Null);
        Assert.That(logoutResponse.Message, Is.EqualTo("Logout success"));
    }
}