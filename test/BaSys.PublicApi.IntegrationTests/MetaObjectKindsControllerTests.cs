using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BaSys.Common.Infrastructure;
using BaSys.Host;
using BaSys.Metadata.Models;
using BaSys.PublicApi.Tests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace BaSys.PublicApi.Tests;

[TestFixture]
public class MetaObjectKindsControllerTests : TestBase
{
    [Test]
    public async Task GetMetaObjectKindsTest()
    {
        Client!.DefaultRequestHeaders.Authorization = await GetAuthToken("admin@mail.ru", "111111", "db1");
        
        var response = await Client!.GetAsync("/api/public/v1/MetaObjectKinds");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var content = await response.Content.ReadFromJsonAsync<ResultWrapper<IEnumerable<MetaObjectKind>>>();
    }
}