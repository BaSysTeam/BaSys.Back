using System.Net.Http.Headers;
using System.Net.Http.Json;
using BaSys.Host;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace BaSys.PublicApi.Tests.Infrastructure;

public class TestBase
{
    protected WebApplicationFactory<Program>? Application;
    protected HttpClient? Client;
    
    [SetUp]
    public void Init()
    {
        Application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json");
                });
            });
        Client = Application.CreateClient();
    }
    
    [TearDown]
    public void Cleanup()
    {
        Application?.Dispose();
        Client?.Dispose();
    }
    
    protected async Task<AuthenticationHeaderValue?> GetAuthToken(string login, string password, string dbName)
    {
        var response = await Client!.GetAsync($"api/public/v1/auth?login={login}&password={password}&dbId={dbName}");
        var content = await response.Content.ReadFromJsonAsync<TokenModel>();

        if (string.IsNullOrEmpty(content?.Token))
            return null;
        
        return new AuthenticationHeaderValue("Bearer", content.Token);
    }
}