using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fin.Api.DTO;
using Fin.Application.UseCases;
using Fin.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Fin.Api.IntegrationTests;

public class UpdateTransferUseCaseIntegrationTests
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
    public async Task GivenATransfer_WhenUpdateTransfer_ThenShouldUpdateTransfer()
    {
        // Arrange
        var loginHttpResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = "user@email.com",
            Password = "12345678"
        });
        var loginResponse = await loginHttpResponse.Content.ReadFromJsonAsync<LoginResponse>();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);

        // Act & Assert
        var putResponse = await _client.PutAsJsonAsync("/api/transfers/51", new UpdateTransferRequest
        {
            Id = 51,
            TransactionId = 1680,
            Date = new DateOnly(2025, 8, 23),
            Description = "transfer87 new description",
            Amount = 5,
            FromAccountId = 16,
            ToAccountId = 62,
            IsRecurrence = false
        });
        Assert.That(putResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var httpResponseMessage = await _client.GetAsync($"/api/months/year/2025/month/8/accounts/16,62");
        Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.NotNull(httpResponseMessage);
        var monthResponse = await httpResponseMessage.Content.ReadFromJsonAsync<GetMonthResponse>();
        Assert.That(monthResponse, Is.Not.Null);

        var transaction = monthResponse.DayTransactions
            .SelectMany(dt => dt.Transactions)
            .FirstOrDefault(t => t.Id == 1680);
        Assert.That(transaction, Is.Not.Null);
        Assert.That(transaction.Description, Is.EqualTo("transfer87 new description"));
    }
    
    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}