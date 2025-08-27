using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fin.Application.UseCases;

namespace Fin.Api.IntegrationTests.Base;

public abstract class IntegrationTestBase
{
    protected HttpClient Client = null!;
    protected IntegrationTestWebApplicationFactory Factory = null!;

    [SetUp]
    public async Task InitializeAsync()
    {
        Factory = new IntegrationTestWebApplicationFactory();
        await Factory.InitializeAsync();
        Client = Factory.CreateClient();
    }

    [TearDown]
    public async Task DisposeAsync()
    {
        Client.Dispose();
        await Factory.StopAsync();
        await Factory.DisposeAsync();
    }

    protected async Task<string> LoginAndSetAccessTokenAsync()
    {
        var loginHttpResponse = await Client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = "user@email.com",
            Password = "12345678"
        });
        
        var loginResponse = await loginHttpResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse!.AccessToken);
        
        return loginResponse.AccessToken;
    }
}
