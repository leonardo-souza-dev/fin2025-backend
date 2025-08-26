using System.Net;
using System.Net.Http.Json;
using Fin.Application.UseCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Fin.Infrastructure.Data;

namespace Fin.Api.IntegrationTests;

public class UpdateTransferUseCaseIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task GivenATransfer_WhenUpdateTransfer_ThenShouldUpdateTransfer()
    {
        // Arrange
        await SetAccessTokenAsync();

        // Get the actual IDs from the seeded data
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FinDbContext>();
        
        var transfer = await dbContext.Transfers.FirstAsync();
        var payment = await dbContext.Payments.FirstAsync(p => p.Id == transfer.PaymentFromId);
        var fromAccount = await dbContext.Accounts.FirstAsync();
        var toAccount = await dbContext.Accounts.Skip(1).FirstAsync();

        // Act & Assert
        var putResponse = await Client.PutAsJsonAsync($"/api/transfers/{transfer.Id}", new UpdateTransferRequest
        {
            Id = transfer.Id,
            PaymentId = payment.Id,
            Date = new DateOnly(2025, 8, 23),
            Description = "transfer87 new description",
            Amount = 5,
            FromAccountId = fromAccount.Id!.Value,
            ToAccountId = toAccount.Id!.Value,
            IsRecurrence = false
        });
        Assert.That(putResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var httpResponseMessage = await Client.GetAsync($"/api/months/year/2025/month/8/accounts/{fromAccount.Id},{toAccount.Id}");
        Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.NotNull(httpResponseMessage);
        var monthResponse = await httpResponseMessage.Content.ReadFromJsonAsync<GetMonthResponse>();
        Assert.That(monthResponse, Is.Not.Null);

        var updatedPayment = monthResponse.DayPayments
            .SelectMany(dt => dt.Payments)
            .FirstOrDefault(t => t.Id == payment.Id);
        Assert.That(updatedPayment, Is.Not.Null);
        Assert.That(updatedPayment.Description, Is.EqualTo("transfer87 new description"));
    }
}