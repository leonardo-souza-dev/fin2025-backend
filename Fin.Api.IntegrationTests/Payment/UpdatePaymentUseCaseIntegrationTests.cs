using System.Net;
using System.Net.Http.Json;
using Fin.Api.IntegrationTests.Base;
using Fin.Application.UseCases;
using System.Net;
using System.Net.Http.Json;
using Fin.Api.IntegrationTests.Base;
using Fin.Application.UseCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Fin.Infrastructure.Data;

namespace Fin.Api.IntegrationTests.Payment;

public class UpdatePaymentUseCaseIntegrationTests : IntegrationTestBase
{
    private List<int?> _accountIds;
    
    [SetUp]
    public async Task SetUp()
    {
        await base.InitializeAsync();
        //var createAccount = await Client.
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FinDbContext>();
        _accountIds = dbContext.Accounts.Select(x => x.Id).ToList();
    }
    
    [Test]
    public async Task GivenAValidRequest_MustUpdatePayment()
    {
        // Arrange
        _ = await LoginAndSetAccessTokenAsync();
        
        var accountId1 = _accountIds[0].Value;
        var accountId2 = _accountIds[1].Value;

        var createPaymentPostResponse = await Client.PostAsJsonAsync("/api/payments", new CreatePaymentRequest
        {
            Date = new DateOnly(2050, 01, 01),
            Description = "payment for integration tests",
            FromAccountId = accountId1,
            Amount = -100
        });
        Assert.True(createPaymentPostResponse.IsSuccessStatusCode);
        var createPaymentResponse = await createPaymentPostResponse.Content.ReadFromJsonAsync<CreatePaymentResponse>();
        // Act
        var actual = await Client.PutAsJsonAsync($"/api/payments/{createPaymentResponse.Id}", new UpdatePaymentRequest
        {
            Id = createPaymentResponse.Id,
            Date = new DateOnly(2050, 1, 2),
            Description = "payment for integration tests updated",
            FromAccountId = accountId2,
            Amount = -101
        });

        // Assert
        Assert.That(actual.IsSuccessStatusCode);
        Assert.That(actual.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var monthHttpResponseMessage = await Client.GetAsync($"/api/months/year/2050/month/1/accounts/{accountId2}");
        Assert.That(monthHttpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.NotNull(monthHttpResponseMessage);
        var monthResponse = await monthHttpResponseMessage.Content.ReadFromJsonAsync<GetMonthResponse>();
        Assert.That(monthResponse, Is.Not.Null);
        
        var updatedPayment = monthResponse.DayPayments
            .SelectMany(dt => dt.Payments)
            .FirstOrDefault(t => t.Id == createPaymentResponse.Id);
        Assert.Multiple(() =>
        {
            Assert.That(updatedPayment, Is.Not.Null);
            Assert.That(updatedPayment.Date, Is.EqualTo(new DateOnly(2050, 1, 2)));
            Assert.That(updatedPayment.Description, Is.EqualTo("payment for integration tests updated"));
            Assert.That(updatedPayment.FromAccountId, Is.EqualTo(accountId2));
            Assert.That(updatedPayment.Amount, Is.EqualTo(-101));
        });

    }
}