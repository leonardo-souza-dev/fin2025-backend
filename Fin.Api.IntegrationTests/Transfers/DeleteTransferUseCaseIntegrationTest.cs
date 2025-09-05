using Fin.Api.IntegrationTests.Base;
using System.Net;
using Fin.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Fin.Infrastructure.Data;

namespace Fin.Api.IntegrationTests.Transfers;

public class DeleteTransferUseCaseIntegrationTest : IntegrationTestBase
{
    
    private Transfer _transfer;
    
    [SetUp]
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FinDbContext>();
        _transfer = dbContext.Transfers.First();
    }

    [Test]
    public async Task MustDeleteTransfer()
    {
        // Arrange
        _ = await LoginAndSetAccessTokenAsync();
        
        // Act
        var response = await Client.DeleteAsync($"api/transfers/{_transfer.Id}");
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }
    
    
}