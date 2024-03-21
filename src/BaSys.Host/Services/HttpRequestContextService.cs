using BaSys.Common.Enums;
using BaSys.Host.Abstractions;
using BaSys.Host.Infrastructure;
using BaSys.Host.Infrastructure.Interfaces;

namespace BaSys.Host.Services;

public class HttpRequestContextService : IHttpRequestContextService
{
    private readonly IServiceProvider _serviceProvider;
    
    public HttpRequestContextService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public ConnectionItem? GetConnectionItem(DbKinds? dbKind = null)
    {
        var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var userId = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault()?.Value;

        ConnectionItem? item;
        
        // if init work db
        if (IsInitDbRequest(httpContextAccessor.HttpContext?.Request.RouteValues))
        {
            var value = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            int.TryParse(value?.ToString(), out var dbInfoId);
            item = _serviceProvider.GetRequiredService<IDataSourceProvider>().GetConnectionItemByDbInfoId(dbInfoId);
            return item;
        }
        
        // From user
        if (!string.IsNullOrEmpty(userId))
        {
            item = _serviceProvider.GetRequiredService<IDataSourceProvider>().GetCurrentConnectionItemByUser(userId);
            return item;
        }
        
        // auth from Login form
        if (httpContextAccessor.HttpContext?.Request.HasFormContentType == true &&
                 httpContextAccessor.HttpContext?.Request.ContentType != null && 
                 httpContextAccessor.HttpContext?.Request.Form?.TryGetValue("Input.DbName", out var dbId) == true)
        {
            item = _serviceProvider.GetRequiredService<IDataSourceProvider>().GetConnectionItemByDbId(dbId);
            return item;
        }
        
        // auth from auth endpoint
        if (httpContextAccessor.HttpContext?.Request.Path == "/api/auth" &&
            httpContextAccessor.HttpContext?.Request.Query.TryGetValue("dbid", out var val) == true)
        {
            var dbIdParam = val.FirstOrDefault();
            if (string.IsNullOrEmpty(dbIdParam))
                throw new ArgumentException("DbId not set in auth query!");

            item = _serviceProvider.GetRequiredService<IDataSourceProvider>().GetConnectionItemByDbId(dbIdParam);
            return item;
        }
        
        item = _serviceProvider.GetRequiredService<IDataSourceProvider>().GetDefaultConnectionItem(dbKind);
        
        return item;
    }

    private bool IsInitDbRequest(RouteValueDictionary? requestRouteValues)
    {
        if (requestRouteValues?.Any() != true)
            return false;
        
        if (!requestRouteValues.Any(x => x.Key == "action" && x.Value?.ToString() == "InitDb"))
            return false;
        
        if (!requestRouteValues.Any(x => x.Key == "controller" && x.Value?.ToString() == "WorkDb"))
            return false;
        
        if (!requestRouteValues.Any(x => x.Key == "id"))
            return false;

        return true;
    }
}