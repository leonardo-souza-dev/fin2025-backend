using System.Net;
using System.Net.Http.Json;
using Fin.Api.IntegrationTests.Base;
using Fin.Application.UseCases.Configs;
using Fin.Application.UseCases.Months;
using Fin.Application.UseCases.Payments;
using Fin.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Fin.Infrastructure.Data;

namespace Fin.Api.IntegrationTests.Configs;

public class UpdateConfigUseCaseIntegrationTests : IntegrationTestBase
{
    private Config _config;
    
    [SetUp]
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FinDbContext>();
        _config = dbContext.Configs.First();
    }
    
    [Test]
    public async Task GivenAValidRequest_MustEditPayment()
    {
        // Arrange
        _ = await LoginAndSetAccessTokenAsync();
        
        // Act
        var actual = await Client.PutAsJsonAsync($"/api/configs/{_config.Id}", new UpdateConfigRequest
        {
            Id = _config.Id,
            Key = _config.Key,
            Value = _config.Value + "b"
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actual.IsSuccessStatusCode);
            Assert.That(actual.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });

        var getConfigsResponse = await Client.GetAsync($"/api/configs");
        Assert.That(getConfigsResponse, Is.Not.Null);
        Assert.That(getConfigsResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var response = await getConfigsResponse.Content.ReadFromJsonAsync<IEnumerable<Config>>();
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Count, Is.GreaterThan(0));
        Assert.That(response.First(x => x.Id == _config.Id).Value, Is.EqualTo(_config.Value + "b"));

    }
}