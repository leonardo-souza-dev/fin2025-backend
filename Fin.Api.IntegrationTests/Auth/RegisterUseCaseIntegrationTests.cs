using System.Net;
using System.Net.Http.Json;
using Fin.Api.IntegrationTests.Base;
using Fin.Application.UseCases.Auth;

namespace Fin.Api.IntegrationTests.Auth;

public class RegisterUseCaseIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task MustRegister()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Email = "user-integration-test@email.com",
            Password = "password12345",
        };
        
        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);
        
        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(async () =>
        {
            Assert.That(response.Content, Is.Not.Null);
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var registerResponse = await response.Content.ReadFromJsonAsync<RegisterResponse>();
            Assert.That(registerResponse, Is.Not.Null);
            Assert.That(registerResponse!.Message, Is.EqualTo("Registration successful"));
        });
    }
    
}