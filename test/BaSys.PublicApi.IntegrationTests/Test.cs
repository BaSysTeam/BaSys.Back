using System.Net;
using BaSys.Host;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace BaSys.PublicApi.Tests;

[TestFixture]
public class Test
{
    [Test]
    public async Task RequestTest()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        
        var response = await client.GetAsync("/api/public/v1/publicapitest");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var content = await response.Content.ReadAsStringAsync();
    }
}