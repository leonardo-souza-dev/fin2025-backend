using System.Net;
using System.Net.Http.Json;
using Fin.Api.IntegrationTests.Base;
using Fin.Application.UseCases.BankAccounts;

namespace Fin.Api.IntegrationTests.Accounts;

public class GetAllBankAccountsUseCaseIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task ShouldGetAllBankAccounts()
    {
        // Arrange
        _ = await LoginAndSetAccessTokenAsync();
        
        // Act
        var response = await Client.GetAsync("/api/bank-accounts");
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var entities = await response.Content.ReadFromJsonAsync<GetAllBankAccountsResponse>();
        Assert.That(entities, Is.Not.Null);
        Assert.That(entities, Is.Not.Empty);
    }
    
}