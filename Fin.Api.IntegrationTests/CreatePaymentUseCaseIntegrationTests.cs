using System.Net;
using System.Net.Http.Json;
using Fin.Application.UseCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Fin.Infrastructure.Data;

namespace Fin.Api.IntegrationTests;

public class CreatePaymentUseCaseIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task GivenAPayment_WhenCreate_ThenShouldCreate()
    {
        // Arrange
        _ = await LoginAndSetAccessTokenAsync();

        // Get the actual account ID from the seeded data
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FinDbContext>();
        var fromAccount = await dbContext.Accounts.FirstAsync();

        // Act
        var postResponse = await Client.PostAsJsonAsync("/api/payments", new CreatePaymentRequest
        {
            Date = new DateOnly(2050, 1, 1),
            Description = "payment test from integration tests",
            Amount = -100,
            FromAccountId = fromAccount.Id!.Value
        });
        
        // Assert
        Assert.That(postResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var createPaymentResponse = await postResponse.Content.ReadFromJsonAsync<CreatePaymentResponse>();
        Assert.That(createPaymentResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(createPaymentResponse.Id, Is.GreaterThan(0));
            Assert.That(createPaymentResponse.Date, Is.EqualTo(new DateOnly(2050, 1, 1)));
            Assert.That(createPaymentResponse.Description, Is.EqualTo("payment test from integration tests"));
            Assert.That(createPaymentResponse.Amount, Is.EqualTo(-100));
            Assert.That(createPaymentResponse.FromAccountId, Is.EqualTo(fromAccount.Id!.Value));
        });
    }
}