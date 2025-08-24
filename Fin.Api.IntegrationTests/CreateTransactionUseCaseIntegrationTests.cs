using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fin.Api.DTO;
using Fin.Application.UseCases;
using Fin.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Fin.Api.IntegrationTests;

public class CreateTransactionUseCaseIntegrationTests
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
    public async Task GivenATransaction_WhenCreate_ThenShouldCreate()
    {
        // Arrange
        await SetAccessToken();

        // Act & Assert
        var postResponse = await _client.PostAsJsonAsync("/api/transactions", new CreateTransactionRequest
        {
            Date = new DateOnly(2050, 1, 1),
            Description = "transaction test from integration tests",
            Amount = -100,
            FromAccountId = 16
        });
        Assert.That(postResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var createTransactionResponse = await postResponse.Content.ReadFromJsonAsync<CreateTransactionResponse>();
        Assert.That(createTransactionResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(createTransactionResponse.Id, Is.GreaterThan(0));
            Assert.That(createTransactionResponse.Date, Is.EqualTo(new DateOnly(2050, 1, 1)));
            Assert.That(createTransactionResponse.Description, Is.EqualTo("transaction test from integration tests"));
            Assert.That(createTransactionResponse.Amount, Is.EqualTo(-100));
            Assert.That(createTransactionResponse.FromAccountId, Is.EqualTo(16));
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