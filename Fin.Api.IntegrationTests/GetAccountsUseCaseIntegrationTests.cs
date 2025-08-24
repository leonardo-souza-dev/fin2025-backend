using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fin.Api.DTO;
using Fin.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Fin.Api.IntegrationTests;

public class GetAccountsUseCaseIntegrationTests
{
    private HttpClient _client = null!;
    private WebApplicationFactory<Program> _factory = null!;
    
    [SetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    private async Task SetAccessToken()
    {
        var loginHttpResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = "user@email.com",
            Password = "12345678"
        });
        var loginResponse = await loginHttpResponse.Content.ReadFromJsonAsync<LoginResponse>();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
    }

    [Test]
    public async Task ShouldGetAllActiveAccounts()
    {
        // Arrange
        await SetAccessToken();

        // Act
        var httpResponseMessage = await _client.GetAsync("/api/accounts");
        
        // Assert
        Assert.That(httpResponseMessage, Is.Not.Null);
        Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var getAccountsResponse = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<Account>>();
        Assert.That(getAccountsResponse, Is.Not.Null);
        Assert.That(getAccountsResponse.Count, Is.GreaterThan(0));
    }
    
    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}