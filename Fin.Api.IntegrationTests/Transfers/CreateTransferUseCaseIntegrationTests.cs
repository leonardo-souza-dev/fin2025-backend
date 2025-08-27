using Fin.Domain.Entities;
using System.Net.Http.Json;
using Fin.Api.IntegrationTests.Base;
using Fin.Api.IntegrationTests.Base;
using Fin.Application.UseCases;
using System.Net;
using System.Net.Http.Json;
using Fin.Api.IntegrationTests.Base;
using Fin.Application.UseCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Fin.Infrastructure.Data;

namespace Fin.Api.IntegrationTests.Transfers;

public class CreateTransferUseCaseIntegrationTests : IntegrationTestBase
{
    private Account _fromAccount;
    private Account _toAccount;
    
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FinDbContext>();
        _fromAccount = dbContext.Accounts.First();
        _toAccount = dbContext.Accounts.Skip(1).First();
    }
    [Test]
    public async Task GivenValidRequest_MustCreateTransfer()
    {
        // Arrange
        _ = await LoginAndSetAccessTokenAsync();
        
        // Act
        var response = await Client.PostAsJsonAsync($"/api/transfers/", new CreateTransferRequest
        {
            Date = new DateOnly(2050, 6, 6),
            Description = "new descriptions transfer",
            FromAccountId = _fromAccount.Id.Value,
            Amount = 10000,
            ToAccountId = _toAccount.Id.Value,
            IsRecurrence = false
        });
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }
}