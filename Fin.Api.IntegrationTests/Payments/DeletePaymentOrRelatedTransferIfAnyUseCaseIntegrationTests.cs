using Fin.Api.IntegrationTests.Base;
using System.Net;
using Fin.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Fin.Infrastructure.Data;

namespace Fin.Api.IntegrationTests.Payments;

public class DeletePaymentOrRelatedTransferIfAnyUseCaseIntegrationTests : IntegrationTestBase
{
    
    private Payment _payment;
    
    [SetUp]
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FinDbContext>();
        _payment = dbContext.Payments.First();
    }
    
    [Test]
    public async Task DeletePaymentOrRelatedTransferIfAny()
    {
        // Arrange
        _ = await LoginAndSetAccessTokenAsync();
        
        // Act
        var response = await Client.DeleteAsync($"/api/payments/{_payment.Id}");
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }
}