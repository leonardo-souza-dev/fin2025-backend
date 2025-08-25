using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fin.Api.DTO;
using Fin.Application.UseCases;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Fin.Api.IntegrationTests;

public class CreatePaymentUseCaseIntegrationTests
{
    private HttpClient _client = null!;
    private WebApplicationFactory<Program> _factory = null!;
    
    [SetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [Test]
    public async Task GivenAPayment_WhenCreate_ThenShouldCreate()
    {
        // Arrange
        await SetAccessToken();

        // Act & Assert
        var postResponse = await _client.PostAsJsonAsync("/api/payments", new CreatePaymentRequest
        {
            Date = new DateOnly(2050, 1, 1),
            Description = "payment test from integration tests",
            Amount = -100,
            FromAccountId = 16
        });
        Assert.That(postResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var createPaymentResponse = await postResponse.Content.ReadFromJsonAsync<CreatePaymentResponse>();
        Assert.That(createPaymentResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(createPaymentResponse.Id, Is.GreaterThan(0));
            Assert.That(createPaymentResponse.Date, Is.EqualTo(new DateOnly(2050, 1, 1)));
            Assert.That(createPaymentResponse.Description, Is.EqualTo("payment test from integration tests"));
            Assert.That(createPaymentResponse.Amount, Is.EqualTo(-100));
            Assert.That(createPaymentResponse.FromAccountId, Is.EqualTo(16));
        });
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
    
    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}